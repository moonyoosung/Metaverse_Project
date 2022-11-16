using BeliefEngine.HealthKit;
using MindPlus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class HealthManager : MonoBehaviour, PostHealthDataNetCommand.IEventHandler
{
    [System.Serializable]
    public class Record
    {
        public string time;
        public string count;
        public string unit;
    }
    public DateTime systemTime;
    private DateTime lastDBDateTime;
    private APIManager aPIManager;
    private PluginManager pluginManager;
    private LocalPlayerData playerData;
    public HealthActivityPair HealthActivityPair { private set; get; }

    public void Initialize(APIManager aPIManager, AccountManager accountManager, PluginManager pluginManager, ResourceManager resourceManager)
    {
        this.aPIManager = aPIManager;
        this.playerData = accountManager.PlayerData;
        this.pluginManager = pluginManager;
        this.HealthActivityPair = resourceManager.HealthActivityPair;
        aPIManager.ResisterEvent(this);
        if (PlayerPrefs.HasKey("lastDBDateTime"))
        {
            Debug.Log("=======================LastDBDateTimeHASKey===================");
            lastDBDateTime = DateTime.ParseExact(PlayerPrefs.GetString("lastDBDateTime"), Format.DateFormat, GameManager.Instance.CultureInfo);
        }
        else
        {

            lastDBDateTime = DateTime.Now.AddMonths(-6);
            Debug.Log("=======================LastDBDateTime : " + lastDBDateTime.ToString("yyyy-MM-dd"));
        }
        StartCoroutine(AutoReadHealthData());
    }

    private IEnumerator AutoReadHealthData()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            //Debug.Log("TimeCheck : " + (DateTime.Now - systemTime).TotalMinutes);
            if ((DateTime.Now - systemTime).TotalMinutes >= 10)
            {
                systemTime = DateTime.Now;
                if (pluginManager.Handler.IsAuthorized() && playerData.userId != null && playerData.userId != string.Empty)
                {
                    Debug.Log("AutoRead Data");
#if UNITY_IOS
                    yield return ReadHealthData();
#endif
                }
            }
        }
    }
    public IEnumerator ReadHealthData(Double fewDaysAgo = 0)
    {
        Debug.Log("================GetHealthData================");
        IOSPluginHandler pluginHandler = (pluginManager.Handler as IOSPluginHandler);
        List<HKDataType> types = pluginHandler.GetReadDataType();
        bool isComplete = false;

        foreach (var type in types)
        {
            while (isComplete)
            {
                yield return null;
            }
            Debug.Log("================Process " + Enum.GetName(typeof(HKDataType), type) + " =================");
            isComplete = true;

            pluginHandler.ReadData(type, DateTime.Now, fewDaysAgo == 0 ? (DateTime.Now - lastDBDateTime).TotalDays : fewDaysAgo,
                (result, dataType) =>
                {
                    string text = "";
                    string[,] values = new string[result.Count, 1];
                    for (int i = 0; i < values.GetLength(0); i++)
                    {
                        values[i, 0] = result[i];
                        text += result[i] + "\n";
                    }

                    List<Record> records = HealthStringSerialize(dataType, result, out DateTime start, out DateTime end);
                    if (records.Count > 0)
                    {
                        PostHealth(dataType, start, end, records.ToArray());

                        WriteCSVData(dataType, end, new string[] { dataType.ToString() }, values, () =>
                        {
                            isComplete = false;
                            Debug.Log("===================isComplete============>" + isComplete);
                        });
                    }
                    else
                    {
                        isComplete = false;
                        Debug.Log("===================NoRecords============>" + records.Count);
                    }
                });
        }
        yield return null;
        this.lastDBDateTime = DateTime.Now;
        PlayerPrefs.SetString("lastDBDateTime", lastDBDateTime.ToString(Format.DateFormat, GameManager.Instance.CultureInfo));
    }

    private void PostHealth(HKDataType hKDataType, DateTime startTime, DateTime endTime, Record[] records)
    {
        string healthId = ConvertHKDataTypeAsHealthId(hKDataType);
        aPIManager.PostAsync(new PostHealthDataNetCommand(), string.Format("/users/{0}/health", playerData.userId),
            new JObject
            {
                { "healthId" , healthId},
                { "startTime", startTime.ToString(Format.DateFormat)},
                { "endTime", endTime.ToString(Format.DateFormat)},
                { "fileName", string.Format("{0}_{1:yyyyMMddHHmmss}.csv", healthId, endTime)},
                { "records", JArray.FromObject(records)}
            });
    }
    private void WriteCSVData(HKDataType hKDataType, DateTime end, string[] titles, string[,] values, Action onComplete = null)
    {
        Debug.Log("=========================FileStream===================");
        string healthId = ConvertHKDataTypeAsHealthId(hKDataType);
        string filePath = Application.persistentDataPath + "/" + healthId + ".csv";
        // 1. ?????? ????????
        //FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        // 2. ???? ???? ????
        StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);

        //StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("euc-kr"));

        // 3. ???? ?????? ?????? ????
        string ti = "";
        for (int i = 0; i < titles.Length; i++)
        {
            ti += titles[i];

            if (i < titles.Length - 1)
            {
                ti += ",";
            }
        }
        sw.WriteLine(ti);

        // 4. ?? ?????? ?????? ????
        for (int i = 0; i < values.GetLength(0); i++)
        {
            string vals = "";

            for (int j = 0; j < values.GetLength(1); j++)
            {
                vals += values[i, j].ToString();

                if (j < values.GetLength(1) - 1)
                {
                    vals += ",";
                }
            }
            sw.WriteLine(vals);
        }
        Debug.Log("=========================CompleteStream===================");
        // 5. ?????? ????
        sw.Flush();
        sw.Close();
        //fs.Close();
        if (!File.Exists(filePath))
        {
            Debug.LogError("Not Found File");
        }
        FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamReader sr = new StreamReader(fileStream, Encoding.UTF8);
        Debug.Log("=================StreamReader===============\n" + sr.ReadToEnd() + "\n=================================");
        aPIManager.UploadCSVFile(fileStream, string.Format("users/{0}/{1}/{1}_{2}.csv", playerData.userId, healthId, string.Format("{0:yyyyMMddHHmmss}", end))
            , () =>
            {
                onComplete?.Invoke();
                fileStream.Close();
                sr.Close();
            });
        Debug.Log("Upload Csv File Bucket");
    }
    public List<Record> HealthStringSerialize(HKDataType hKDataType, List<string> result, out DateTime start, out DateTime end)
    {
        start = new DateTime();
        end = new DateTime();

        List<Record> records = new List<Record>();
        for (int i = 0; i < result.Count; i++)
        {
            // 0 => ????????, ????????
            // 1 => ??, ????
            // 2 => ??????, ???? ??
            string[] categorys = result[i].Split('/');

            string dateTime = categorys[0].Split(',')[1];
            if (i == 0)
            {
                start = DateTime.ParseExact(dateTime, Format.DateFormat, GameManager.Instance.CultureInfo);
            }
            if (i == result.Count - 1)
            {
                end = DateTime.ParseExact(dateTime, Format.DateFormat, GameManager.Instance.CultureInfo);
            }
            string dataValue = categorys[1].Split(',')[0];
            string dataUnit = categorys[1].Split(',')[1];
            if (categorys.Length == 3)
            {
                dataUnit += "/" + categorys[2];
            }

            Record record = new Record();
            record.time = dateTime;
            record.count = dataValue;
            record.unit = dataUnit;
            records.Add(record);
        }
        return records;
    }
    public void OnFailedPostHealth(NetworkMessage message)
    {
        Debug.Log(message.body);
    }

    public void OnSuccessPostHealth(NetworkMessage message)
    {
        Debug.Log(message.body);
        Alarm alarm = JsonUtility.FromJson<Alarm>(message.body);
        if (alarm.pushAlarms == null || alarm.pushAlarms.Length <= 0)
        {
            return;
        }
        NotifyView notify = UIView.Get<NotifyView>();
        foreach (var notifyData in alarm.pushAlarms)
        {
            notifyData.showingTime = 3f;
            notify.PushNotify(notifyData);
        }
    }

    public bool IsIntegerUnit(HKDataType hKDataType)
    {
        switch (hKDataType)
        {
            case HKDataType.HKQuantityTypeIdentifierHeartRateVariabilitySDNN: return false;
            case HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning: return false;
            default: return true;
        }
    }
    public string GetAverageText(HKDataType hKDataType)
    {
        switch (hKDataType)
        {
            case HKDataType.HKQuantityTypeIdentifierStepCount: return "TOTAL";
            case HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning: return "TOTAL";
            case HKDataType.HKQuantityTypeIdentifierAppleExerciseTime: return "TOTAL";
            default: return "AVERAGE";
        }
    }
    public bool IsAverageType(HKDataType hKDataType)
    {
        switch (hKDataType)
        {
            case HKDataType.HKQuantityTypeIdentifierStepCount: return false;
            case HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning: return false;
            case HKDataType.HKQuantityTypeIdentifierAppleExerciseTime: return false;
            default: return true;
        }
    }

    public string ConvertHKDataTypeAsHealthId(HKDataType hkType)
    {
        string result = "";
        switch (hkType)
        {
            case HKDataType.HKQuantityTypeIdentifierStepCount: result = "STC"; return result;
            case HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning: result = "WRD"; return result;
            case HKDataType.HKQuantityTypeIdentifierAppleExerciseTime: result = "EXT"; return result;
            case HKDataType.HKQuantityTypeIdentifierHeartRate: result = "AHR"; return result;
            case HKDataType.HKQuantityTypeIdentifierRestingHeartRate: result = "RHR"; return result;
            case HKDataType.HKQuantityTypeIdentifierHeartRateVariabilitySDNN: result = "HRV"; return result;

            #region "???????? ???? ?????? ????"
            case HKDataType.HKQuantityTypeIdentifierRespiratoryRate: break;

            case HKDataType.HKQuantityTypeIdentifierBodyMassIndex:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyFatPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierHeight:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyMass:
                break;
            case HKDataType.HKQuantityTypeIdentifierLeanBodyMass:
                break;
            case HKDataType.HKQuantityTypeIdentifierWaistCircumference:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceCycling:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceWheelchair:
                break;
            case HKDataType.HKQuantityTypeIdentifierBasalEnergyBurned:
                break;
            case HKDataType.HKQuantityTypeIdentifierActiveEnergyBurned:
                break;
            case HKDataType.HKQuantityTypeIdentifierFlightsClimbed:
                break;
            case HKDataType.HKQuantityTypeIdentifierNikeFuel:
                break;
            case HKDataType.HKQuantityTypeIdentifierPushCount:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceSwimming:
                break;
            case HKDataType.HKQuantityTypeIdentifierSwimmingStrokeCount:
                break;
            case HKDataType.HKQuantityTypeIdentifierVO2Max:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceDownhillSnowSports:
                break;
            case HKDataType.HKQuantityTypeIdentifierAppleStandTime:
                break;
            case HKDataType.HKQuantityTypeIdentifierAppleMoveTime:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyTemperature:
                break;
            case HKDataType.HKQuantityTypeIdentifierBasalBodyTemperature:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodPressureSystolic:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodPressureDiastolic:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingHeartRateAverage:
                break;
            case HKDataType.HKQuantityTypeIdentifierOxygenSaturation:
                break;
            case HKDataType.HKQuantityTypeIdentifierPeripheralPerfusionIndex:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodGlucose:
                break;
            case HKDataType.HKQuantityTypeIdentifierNumberOfTimesFallen:
                break;
            case HKDataType.HKQuantityTypeIdentifierElectrodermalActivity:
                break;
            case HKDataType.HKQuantityTypeIdentifierInhalerUsage:
                break;
            case HKDataType.HKQuantityTypeIdentifierInsulinDelivery:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodAlcoholContent:
                break;
            case HKDataType.HKQuantityTypeIdentifierForcedVitalCapacity:
                break;
            case HKDataType.HKQuantityTypeIdentifierForcedExpiratoryVolume1:
                break;
            case HKDataType.HKQuantityTypeIdentifierPeakExpiratoryFlowRate:
                break;
            case HKDataType.HKQuantityTypeIdentifierEnvironmentalAudioExposure:
                break;
            case HKDataType.HKQuantityTypeIdentifierHeadphoneAudioExposure:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatTotal:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatPolyunsaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatMonounsaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatSaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCholesterol:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySodium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCarbohydrates:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFiber:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySugar:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryEnergyConsumed:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryProtein:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminA:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminB6:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminB12:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminC:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminD:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminE:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminK:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCalcium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryIron:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryThiamin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryRiboflavin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryNiacin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFolate:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryBiotin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPantothenicAcid:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPhosphorus:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryIodine:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryMagnesium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryZinc:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySelenium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCopper:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryManganese:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryChromium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryMolybdenum:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryChloride:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPotassium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCaffeine:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryWater:
                break;
            case HKDataType.HKQuantityTypeIdentifierSixMinuteWalkTestDistance:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingStepLength:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingAsymmetryPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingDoubleSupportPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierStairAscentSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierStairDescentSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierUVExposure:
                break;
            case HKDataType.HKCategoryTypeIdentifierSleepAnalysis:
                break;
            case HKDataType.HKCategoryTypeIdentifierAppleStandHour:
                break;
            case HKDataType.HKCategoryTypeIdentifierCervicalMucusQuality:
                break;
            case HKDataType.HKCategoryTypeIdentifierOvulationTestResult:
                break;
            case HKDataType.HKCategoryTypeIdentifierMenstrualFlow:
                break;
            case HKDataType.HKCategoryTypeIdentifierIntermenstrualBleeding:
                break;
            case HKDataType.HKCategoryTypeIdentifierSexualActivity:
                break;
            case HKDataType.HKCategoryTypeIdentifierMindfulSession:
                break;
            case HKDataType.HKCategoryTypeIdentifierHighHeartRateEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowHeartRateEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierIrregularHeartRhythmEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierToothbrushingEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierPregnancy:
                break;
            case HKDataType.HKCategoryTypeIdentifierLactation:
                break;
            case HKDataType.HKCategoryTypeIdentifierContraceptive:
                break;
            case HKDataType.HKCategoryTypeIdentifierEnvironmentalAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeadphoneAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierHandwashingEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowCardioFitnessEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierAbdominalCramps:
                break;
            case HKDataType.HKCategoryTypeIdentifierAcne:
                break;
            case HKDataType.HKCategoryTypeIdentifierAppetiteChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierBladderIncontinence:
                break;
            case HKDataType.HKCategoryTypeIdentifierBloating:
                break;
            case HKDataType.HKCategoryTypeIdentifierBreastPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierChestTightnessOrPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierChills:
                break;
            case HKDataType.HKCategoryTypeIdentifierConstipation:
                break;
            case HKDataType.HKCategoryTypeIdentifierCoughing:
                break;
            case HKDataType.HKCategoryTypeIdentifierDiarrhea:
                break;
            case HKDataType.HKCategoryTypeIdentifierDizziness:
                break;
            case HKDataType.HKCategoryTypeIdentifierDrySkin:
                break;
            case HKDataType.HKCategoryTypeIdentifierFainting:
                break;
            case HKDataType.HKCategoryTypeIdentifierFatigue:
                break;
            case HKDataType.HKCategoryTypeIdentifierFever:
                break;
            case HKDataType.HKCategoryTypeIdentifierGeneralizedBodyAche:
                break;
            case HKDataType.HKCategoryTypeIdentifierHairLoss:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeadache:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeartburn:
                break;
            case HKDataType.HKCategoryTypeIdentifierHotFlashes:
                break;
            case HKDataType.HKCategoryTypeIdentifierLossOfSmell:
                break;
            case HKDataType.HKCategoryTypeIdentifierLossOfTaste:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowerBackPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierMemoryLapse:
                break;
            case HKDataType.HKCategoryTypeIdentifierMoodChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierNausea:
                break;
            case HKDataType.HKCategoryTypeIdentifierNightSweats:
                break;
            case HKDataType.HKCategoryTypeIdentifierPelvicPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierRapidPoundingOrFlutteringHeartbeat:
                break;
            case HKDataType.HKCategoryTypeIdentifierRunnyNose:
                break;
            case HKDataType.HKCategoryTypeIdentifierShortnessOfBreath:
                break;
            case HKDataType.HKCategoryTypeIdentifierSinusCongestion:
                break;
            case HKDataType.HKCategoryTypeIdentifierSkippedHeartbeat:
                break;
            case HKDataType.HKCategoryTypeIdentifierSleepChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierSoreThroat:
                break;
            case HKDataType.HKCategoryTypeIdentifierVaginalDryness:
                break;
            case HKDataType.HKCategoryTypeIdentifierVomiting:
                break;
            case HKDataType.HKCategoryTypeIdentifierWheezing:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierBiologicalSex:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierBloodType:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierDateOfBirth:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierFitzpatrickSkinType:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierWheelchairUse:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierActivityMoveMode:
                break;
            case HKDataType.HKCorrelationTypeIdentifierBloodPressure:
                break;
            case HKDataType.HKCorrelationTypeIdentifierFood:
                break;
            case HKDataType.HKDocumentTypeIdentifierCDA:
                break;
            case HKDataType.HKWorkoutTypeIdentifier:
                break;
            case HKDataType.NUM_TYPES:
                break;
                #endregion

        }

        Debug.LogError("Not Convert Set HKDataType : " + hkType.ToString() + " as a HealthID");
        return result;
    }
    public string GetChallengeCategory(HKDataType hkType)
    {
        switch (hkType)
        {
            #region "InApp"
            #endregion

            #region "OutDoor"
            case HKDataType.HKQuantityTypeIdentifierHeartRate: return StringTable.OutDoor;
            case HKDataType.HKQuantityTypeIdentifierRestingHeartRate: return StringTable.OutDoor;
            case HKDataType.HKQuantityTypeIdentifierHeartRateVariabilitySDNN: return StringTable.OutDoor;
            case HKDataType.HKQuantityTypeIdentifierStepCount: return StringTable.OutDoor;
            case HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning: return StringTable.OutDoor;
            case HKDataType.HKQuantityTypeIdentifierAppleExerciseTime: return StringTable.OutDoor;
            #endregion

            #region "???????? ???? ?????? ????"
            case HKDataType.HKQuantityTypeIdentifierRespiratoryRate: break;

            case HKDataType.HKQuantityTypeIdentifierBodyMassIndex:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyFatPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierHeight:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyMass:
                break;
            case HKDataType.HKQuantityTypeIdentifierLeanBodyMass:
                break;
            case HKDataType.HKQuantityTypeIdentifierWaistCircumference:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceCycling:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceWheelchair:
                break;
            case HKDataType.HKQuantityTypeIdentifierBasalEnergyBurned:
                break;
            case HKDataType.HKQuantityTypeIdentifierActiveEnergyBurned:
                break;
            case HKDataType.HKQuantityTypeIdentifierFlightsClimbed:
                break;
            case HKDataType.HKQuantityTypeIdentifierNikeFuel:
                break;
            case HKDataType.HKQuantityTypeIdentifierPushCount:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceSwimming:
                break;
            case HKDataType.HKQuantityTypeIdentifierSwimmingStrokeCount:
                break;
            case HKDataType.HKQuantityTypeIdentifierVO2Max:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceDownhillSnowSports:
                break;
            case HKDataType.HKQuantityTypeIdentifierAppleStandTime:
                break;
            case HKDataType.HKQuantityTypeIdentifierAppleMoveTime:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyTemperature:
                break;
            case HKDataType.HKQuantityTypeIdentifierBasalBodyTemperature:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodPressureSystolic:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodPressureDiastolic:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingHeartRateAverage:
                break;
            case HKDataType.HKQuantityTypeIdentifierOxygenSaturation:
                break;
            case HKDataType.HKQuantityTypeIdentifierPeripheralPerfusionIndex:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodGlucose:
                break;
            case HKDataType.HKQuantityTypeIdentifierNumberOfTimesFallen:
                break;
            case HKDataType.HKQuantityTypeIdentifierElectrodermalActivity:
                break;
            case HKDataType.HKQuantityTypeIdentifierInhalerUsage:
                break;
            case HKDataType.HKQuantityTypeIdentifierInsulinDelivery:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodAlcoholContent:
                break;
            case HKDataType.HKQuantityTypeIdentifierForcedVitalCapacity:
                break;
            case HKDataType.HKQuantityTypeIdentifierForcedExpiratoryVolume1:
                break;
            case HKDataType.HKQuantityTypeIdentifierPeakExpiratoryFlowRate:
                break;
            case HKDataType.HKQuantityTypeIdentifierEnvironmentalAudioExposure:
                break;
            case HKDataType.HKQuantityTypeIdentifierHeadphoneAudioExposure:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatTotal:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatPolyunsaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatMonounsaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatSaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCholesterol:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySodium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCarbohydrates:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFiber:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySugar:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryEnergyConsumed:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryProtein:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminA:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminB6:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminB12:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminC:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminD:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminE:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminK:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCalcium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryIron:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryThiamin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryRiboflavin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryNiacin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFolate:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryBiotin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPantothenicAcid:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPhosphorus:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryIodine:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryMagnesium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryZinc:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySelenium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCopper:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryManganese:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryChromium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryMolybdenum:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryChloride:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPotassium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCaffeine:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryWater:
                break;
            case HKDataType.HKQuantityTypeIdentifierSixMinuteWalkTestDistance:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingStepLength:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingAsymmetryPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingDoubleSupportPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierStairAscentSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierStairDescentSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierUVExposure:
                break;
            case HKDataType.HKCategoryTypeIdentifierSleepAnalysis:
                break;
            case HKDataType.HKCategoryTypeIdentifierAppleStandHour:
                break;
            case HKDataType.HKCategoryTypeIdentifierCervicalMucusQuality:
                break;
            case HKDataType.HKCategoryTypeIdentifierOvulationTestResult:
                break;
            case HKDataType.HKCategoryTypeIdentifierMenstrualFlow:
                break;
            case HKDataType.HKCategoryTypeIdentifierIntermenstrualBleeding:
                break;
            case HKDataType.HKCategoryTypeIdentifierSexualActivity:
                break;
            case HKDataType.HKCategoryTypeIdentifierMindfulSession:
                break;
            case HKDataType.HKCategoryTypeIdentifierHighHeartRateEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowHeartRateEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierIrregularHeartRhythmEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierToothbrushingEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierPregnancy:
                break;
            case HKDataType.HKCategoryTypeIdentifierLactation:
                break;
            case HKDataType.HKCategoryTypeIdentifierContraceptive:
                break;
            case HKDataType.HKCategoryTypeIdentifierEnvironmentalAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeadphoneAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierHandwashingEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowCardioFitnessEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierAbdominalCramps:
                break;
            case HKDataType.HKCategoryTypeIdentifierAcne:
                break;
            case HKDataType.HKCategoryTypeIdentifierAppetiteChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierBladderIncontinence:
                break;
            case HKDataType.HKCategoryTypeIdentifierBloating:
                break;
            case HKDataType.HKCategoryTypeIdentifierBreastPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierChestTightnessOrPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierChills:
                break;
            case HKDataType.HKCategoryTypeIdentifierConstipation:
                break;
            case HKDataType.HKCategoryTypeIdentifierCoughing:
                break;
            case HKDataType.HKCategoryTypeIdentifierDiarrhea:
                break;
            case HKDataType.HKCategoryTypeIdentifierDizziness:
                break;
            case HKDataType.HKCategoryTypeIdentifierDrySkin:
                break;
            case HKDataType.HKCategoryTypeIdentifierFainting:
                break;
            case HKDataType.HKCategoryTypeIdentifierFatigue:
                break;
            case HKDataType.HKCategoryTypeIdentifierFever:
                break;
            case HKDataType.HKCategoryTypeIdentifierGeneralizedBodyAche:
                break;
            case HKDataType.HKCategoryTypeIdentifierHairLoss:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeadache:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeartburn:
                break;
            case HKDataType.HKCategoryTypeIdentifierHotFlashes:
                break;
            case HKDataType.HKCategoryTypeIdentifierLossOfSmell:
                break;
            case HKDataType.HKCategoryTypeIdentifierLossOfTaste:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowerBackPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierMemoryLapse:
                break;
            case HKDataType.HKCategoryTypeIdentifierMoodChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierNausea:
                break;
            case HKDataType.HKCategoryTypeIdentifierNightSweats:
                break;
            case HKDataType.HKCategoryTypeIdentifierPelvicPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierRapidPoundingOrFlutteringHeartbeat:
                break;
            case HKDataType.HKCategoryTypeIdentifierRunnyNose:
                break;
            case HKDataType.HKCategoryTypeIdentifierShortnessOfBreath:
                break;
            case HKDataType.HKCategoryTypeIdentifierSinusCongestion:
                break;
            case HKDataType.HKCategoryTypeIdentifierSkippedHeartbeat:
                break;
            case HKDataType.HKCategoryTypeIdentifierSleepChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierSoreThroat:
                break;
            case HKDataType.HKCategoryTypeIdentifierVaginalDryness:
                break;
            case HKDataType.HKCategoryTypeIdentifierVomiting:
                break;
            case HKDataType.HKCategoryTypeIdentifierWheezing:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierBiologicalSex:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierBloodType:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierDateOfBirth:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierFitzpatrickSkinType:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierWheelchairUse:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierActivityMoveMode:
                break;
            case HKDataType.HKCorrelationTypeIdentifierBloodPressure:
                break;
            case HKDataType.HKCorrelationTypeIdentifierFood:
                break;
            case HKDataType.HKDocumentTypeIdentifierCDA:
                break;
            case HKDataType.HKWorkoutTypeIdentifier:
                break;
            case HKDataType.NUM_TYPES:
                break;
                #endregion

        }

        Debug.LogError("Not Found HKDataType : " + hkType.ToString() + " In Challenge Category");
        return "";
    }
    public string GetUnit(HKDataType hkType)
    {
        Debug.Log("====================" + hkType.ToString());
        switch (hkType)
        {
            case HKDataType.HKQuantityTypeIdentifierStepCount: return "Steps";
            case HKDataType.HKQuantityTypeIdentifierDistanceWalkingRunning: return "km";
            case HKDataType.HKQuantityTypeIdentifierAppleExerciseTime: return "Min";
            case HKDataType.HKQuantityTypeIdentifierHeartRate: return "ms";
            case HKDataType.HKQuantityTypeIdentifierRestingHeartRate: return "BPM";
            case HKDataType.HKQuantityTypeIdentifierHeartRateVariabilitySDNN: return "BPM";

            #region "???????? ???? ?????? ????"
            case HKDataType.HKQuantityTypeIdentifierRespiratoryRate: break;

            case HKDataType.HKQuantityTypeIdentifierBodyMassIndex:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyFatPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierHeight:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyMass:
                break;
            case HKDataType.HKQuantityTypeIdentifierLeanBodyMass:
                break;
            case HKDataType.HKQuantityTypeIdentifierWaistCircumference:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceCycling:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceWheelchair:
                break;
            case HKDataType.HKQuantityTypeIdentifierBasalEnergyBurned:
                break;
            case HKDataType.HKQuantityTypeIdentifierActiveEnergyBurned:
                break;
            case HKDataType.HKQuantityTypeIdentifierFlightsClimbed:
                break;
            case HKDataType.HKQuantityTypeIdentifierNikeFuel:
                break;
            case HKDataType.HKQuantityTypeIdentifierPushCount:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceSwimming:
                break;
            case HKDataType.HKQuantityTypeIdentifierSwimmingStrokeCount:
                break;
            case HKDataType.HKQuantityTypeIdentifierVO2Max:
                break;
            case HKDataType.HKQuantityTypeIdentifierDistanceDownhillSnowSports:
                break;
            case HKDataType.HKQuantityTypeIdentifierAppleStandTime:
                break;
            case HKDataType.HKQuantityTypeIdentifierAppleMoveTime:
                break;
            case HKDataType.HKQuantityTypeIdentifierBodyTemperature:
                break;
            case HKDataType.HKQuantityTypeIdentifierBasalBodyTemperature:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodPressureSystolic:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodPressureDiastolic:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingHeartRateAverage:
                break;
            case HKDataType.HKQuantityTypeIdentifierOxygenSaturation:
                break;
            case HKDataType.HKQuantityTypeIdentifierPeripheralPerfusionIndex:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodGlucose:
                break;
            case HKDataType.HKQuantityTypeIdentifierNumberOfTimesFallen:
                break;
            case HKDataType.HKQuantityTypeIdentifierElectrodermalActivity:
                break;
            case HKDataType.HKQuantityTypeIdentifierInhalerUsage:
                break;
            case HKDataType.HKQuantityTypeIdentifierInsulinDelivery:
                break;
            case HKDataType.HKQuantityTypeIdentifierBloodAlcoholContent:
                break;
            case HKDataType.HKQuantityTypeIdentifierForcedVitalCapacity:
                break;
            case HKDataType.HKQuantityTypeIdentifierForcedExpiratoryVolume1:
                break;
            case HKDataType.HKQuantityTypeIdentifierPeakExpiratoryFlowRate:
                break;
            case HKDataType.HKQuantityTypeIdentifierEnvironmentalAudioExposure:
                break;
            case HKDataType.HKQuantityTypeIdentifierHeadphoneAudioExposure:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatTotal:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatPolyunsaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatMonounsaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFatSaturated:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCholesterol:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySodium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCarbohydrates:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFiber:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySugar:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryEnergyConsumed:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryProtein:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminA:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminB6:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminB12:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminC:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminD:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminE:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryVitaminK:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCalcium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryIron:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryThiamin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryRiboflavin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryNiacin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryFolate:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryBiotin:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPantothenicAcid:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPhosphorus:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryIodine:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryMagnesium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryZinc:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietarySelenium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCopper:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryManganese:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryChromium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryMolybdenum:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryChloride:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryPotassium:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryCaffeine:
                break;
            case HKDataType.HKQuantityTypeIdentifierDietaryWater:
                break;
            case HKDataType.HKQuantityTypeIdentifierSixMinuteWalkTestDistance:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingStepLength:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingAsymmetryPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierWalkingDoubleSupportPercentage:
                break;
            case HKDataType.HKQuantityTypeIdentifierStairAscentSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierStairDescentSpeed:
                break;
            case HKDataType.HKQuantityTypeIdentifierUVExposure:
                break;
            case HKDataType.HKCategoryTypeIdentifierSleepAnalysis:
                break;
            case HKDataType.HKCategoryTypeIdentifierAppleStandHour:
                break;
            case HKDataType.HKCategoryTypeIdentifierCervicalMucusQuality:
                break;
            case HKDataType.HKCategoryTypeIdentifierOvulationTestResult:
                break;
            case HKDataType.HKCategoryTypeIdentifierMenstrualFlow:
                break;
            case HKDataType.HKCategoryTypeIdentifierIntermenstrualBleeding:
                break;
            case HKDataType.HKCategoryTypeIdentifierSexualActivity:
                break;
            case HKDataType.HKCategoryTypeIdentifierMindfulSession:
                break;
            case HKDataType.HKCategoryTypeIdentifierHighHeartRateEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowHeartRateEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierIrregularHeartRhythmEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierToothbrushingEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierPregnancy:
                break;
            case HKDataType.HKCategoryTypeIdentifierLactation:
                break;
            case HKDataType.HKCategoryTypeIdentifierContraceptive:
                break;
            case HKDataType.HKCategoryTypeIdentifierEnvironmentalAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeadphoneAudioExposureEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierHandwashingEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowCardioFitnessEvent:
                break;
            case HKDataType.HKCategoryTypeIdentifierAbdominalCramps:
                break;
            case HKDataType.HKCategoryTypeIdentifierAcne:
                break;
            case HKDataType.HKCategoryTypeIdentifierAppetiteChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierBladderIncontinence:
                break;
            case HKDataType.HKCategoryTypeIdentifierBloating:
                break;
            case HKDataType.HKCategoryTypeIdentifierBreastPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierChestTightnessOrPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierChills:
                break;
            case HKDataType.HKCategoryTypeIdentifierConstipation:
                break;
            case HKDataType.HKCategoryTypeIdentifierCoughing:
                break;
            case HKDataType.HKCategoryTypeIdentifierDiarrhea:
                break;
            case HKDataType.HKCategoryTypeIdentifierDizziness:
                break;
            case HKDataType.HKCategoryTypeIdentifierDrySkin:
                break;
            case HKDataType.HKCategoryTypeIdentifierFainting:
                break;
            case HKDataType.HKCategoryTypeIdentifierFatigue:
                break;
            case HKDataType.HKCategoryTypeIdentifierFever:
                break;
            case HKDataType.HKCategoryTypeIdentifierGeneralizedBodyAche:
                break;
            case HKDataType.HKCategoryTypeIdentifierHairLoss:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeadache:
                break;
            case HKDataType.HKCategoryTypeIdentifierHeartburn:
                break;
            case HKDataType.HKCategoryTypeIdentifierHotFlashes:
                break;
            case HKDataType.HKCategoryTypeIdentifierLossOfSmell:
                break;
            case HKDataType.HKCategoryTypeIdentifierLossOfTaste:
                break;
            case HKDataType.HKCategoryTypeIdentifierLowerBackPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierMemoryLapse:
                break;
            case HKDataType.HKCategoryTypeIdentifierMoodChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierNausea:
                break;
            case HKDataType.HKCategoryTypeIdentifierNightSweats:
                break;
            case HKDataType.HKCategoryTypeIdentifierPelvicPain:
                break;
            case HKDataType.HKCategoryTypeIdentifierRapidPoundingOrFlutteringHeartbeat:
                break;
            case HKDataType.HKCategoryTypeIdentifierRunnyNose:
                break;
            case HKDataType.HKCategoryTypeIdentifierShortnessOfBreath:
                break;
            case HKDataType.HKCategoryTypeIdentifierSinusCongestion:
                break;
            case HKDataType.HKCategoryTypeIdentifierSkippedHeartbeat:
                break;
            case HKDataType.HKCategoryTypeIdentifierSleepChanges:
                break;
            case HKDataType.HKCategoryTypeIdentifierSoreThroat:
                break;
            case HKDataType.HKCategoryTypeIdentifierVaginalDryness:
                break;
            case HKDataType.HKCategoryTypeIdentifierVomiting:
                break;
            case HKDataType.HKCategoryTypeIdentifierWheezing:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierBiologicalSex:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierBloodType:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierDateOfBirth:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierFitzpatrickSkinType:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierWheelchairUse:
                break;
            case HKDataType.HKCharacteristicTypeIdentifierActivityMoveMode:
                break;
            case HKDataType.HKCorrelationTypeIdentifierBloodPressure:
                break;
            case HKDataType.HKCorrelationTypeIdentifierFood:
                break;
            case HKDataType.HKDocumentTypeIdentifierCDA:
                break;
            case HKDataType.HKWorkoutTypeIdentifier:
                break;
            case HKDataType.NUM_TYPES:
                break;
                #endregion

        }

        Debug.LogError("Not Found Set HKDataType : " + hkType.ToString() + " UnitType");
        return "";
    }
}