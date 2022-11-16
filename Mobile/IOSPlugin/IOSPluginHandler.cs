using BeliefEngine.HealthKit;
using MindPlus;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class IOSPluginHandler : PluginHandler
{
    private HealthStore healthStore;
    private HealthKitDataTypes types;
    private bool isAuthorized = false;
#if UNITY_IOS || UNITY_EDITOR_OSX

    [DllImport("__Internal")]
    private static extern void _ShowAlert(string title, string message);

    public static void ShowAlert(string title, string message)
    {
        _ShowAlert(title, message);
    }
#endif
    public override void Initialize()
    {
        this.healthStore = new GameObject("HealthStore").AddComponent<HealthStore>();
        healthStore.transform.SetParent(transform);
        healthStore.Initialize();

        this.types = Resources.Load<HealthKitDataTypes>("PluginHandler/IOSHealthKitDataTypes");
        if (types == null)
        {
            Debug.Log("Not Found HealthKitDataTypes");
        }
        types.Load();
#if UNITY_IOS && !UNITY_EDITOR
        isAuthorized = CheckPermission();
        Debug.Log("isAuthorized : " + isAuthorized);
#endif

    }

    public override bool CheckPermission()
    {
        
        //Debug.Log("CheckPermission : " + types + " / " + types.data.Values.Count);
        foreach (var pair in types.data)
        {
            if (!pair.Value.read)
            {
                continue;
            }

            HKAuthorizationStatus auth = healthStore.AuthorizationStatusForType(Enum.Parse<HKDataType>(pair.Key));

            if (!pair.Value.writable && pair.Value.read)
            {
                continue;
            }

            if (auth != HKAuthorizationStatus.SharingAuthorized)
            {
                Debug.Log(pair.Key + " : " + auth.ToString() + "\n========================\n" + "Read : " + pair.Value.read + ", " + "Write : " + pair.Value.write + "\n========================\n");
                return false;
            }

        }
        isAuthorized = true;
        return true;
    }

    public override void Authorize(Action<bool> result = null)
    {
        AuthorizationHandler authorizationHandler = new AuthorizationHandler(result);
        healthStore.Authorize(types, authorizationHandler);
    }
    public string GetHKDataTypeName(HKDataType type)
    {
        string key = HealthKitDataTypes.GetIdentifier(type);
        foreach (var data in types.data)
        {
            if(data.Key == key)
            {
                return data.Value.name;
            }
        }

        Debug.LogError("Not Found HKDataType " + type.ToString());
        return "";
    }

    public List<HKDataType> GetReadDataType()
    {
        List<HKDataType> result = new List<HKDataType>();
        foreach (var pair in types.data)
        {
            if (pair.Value.read)
            {
                Debug.Log("===============>ReadDataType : " + pair.Key);
                result.Add(Enum.Parse<HKDataType>(pair.Key));
            }
        }

        return result;
    }
    public void ReadData(HKDataType dataType, DateTimeOffset end, Double fewDaysAgo, Action<List<string>, HKDataType> complete)
    {
        Debug.Log("======================ReadData=========================");
        try
        {
            // for this example, we'll read everything from the past 24 hours
            DateTimeOffset start = end.AddDays(-fewDaysAgo);

            if (dataType <= HKDataType.HKQuantityTypeIdentifierUVExposure)
            {
                // quantity-type
                Debug.Log("reading quantity-type..."+ dataType.ToString());
                ReadQuantityData(dataType, start, end, complete);
            }
            else if (dataType <= HKDataType.HKCategoryTypeIdentifierMindfulSession)
            {
                // category-type
                Debug.Log("reading category-type..." + dataType.ToString());
            }
            else if (dataType <= HKDataType.HKCharacteristicTypeIdentifierWheelchairUse)
            {
                // characteristic-type
                Debug.Log("reading characteristic-type..." + dataType.ToString());
                ReadCharacteristic(dataType, complete);
            }
            else if (dataType <= HKDataType.HKCorrelationTypeIdentifierFood)
            {
                // correlation-type
                Debug.Log("reading correlation-type..." + dataType.ToString());
                ReadCorrelationData(dataType, start, end, complete);
            }
            else if (dataType == HKDataType.HKWorkoutTypeIdentifier)
            {
                // finally, workout-type
                Debug.Log("reading workout-type..." + dataType.ToString());
                ReadWorkoutData(dataType, start, end, complete);
            }
            else
            {
                Debug.LogError(string.Format("data type {0} invalid", HealthKitDataTypes.GetIdentifier(dataType)));
                complete.Invoke(new List<string>(), dataType);
            }
        }
        catch (ArgumentException)
        {
            // they just selected a divider; nothing to worry about
            Debug.LogFormat("{0} unrecognized", dataType.ToString());
            complete.Invoke(new List<string>(), dataType);
        }
    }

    private void ReadQuantityData(HKDataType dataType, DateTimeOffset start, DateTimeOffset end, Action<List<string>, HKDataType> complete)
    {
        string typeName = HealthKitDataTypes.GetIdentifier(dataType);
        Debug.LogFormat("reading {0} from {1} to {2}", typeName, start, end);
        double sum = 0;
        List<string> result = new List<string>();

        this.healthStore.ReadQuantitySamples(dataType, start, end, delegate (List<QuantitySample> samples, Error error)
        {
            if (samples.Count > 0)
            {
                Debug.Log("found " + samples.Count + " samples");
                bool cumulative = (samples[0].quantityType == QuantityType.cumulative);
                DateTime startTime = end.DateTime;
                DateTime endTime = start.DateTime;
                foreach (QuantitySample sample in samples)
                {
                    //result.Add(string.Format("{0}/{1}", sample, sample.quantity.doubleValue));
                    if (cumulative)
                    {
                        sum += Convert.ToInt32(sample.quantity.doubleValue);
                        if (startTime > sample.endDate.DateTime)
                        {
                            startTime = sample.endDate.DateTime;
                        }
                        if (endTime < sample.endDate.DateTime)
                        {
                            endTime = sample.endDate.DateTime;
                        }
                    }
                    else
                    {
                        result.Add(sample.ToString());
                    }
                }

                if (cumulative)
                {
                    if (sum > 0)
                    {
                        result.Add(string.Format("{0},{1}/{2}/{3}", startTime, endTime, typeName, sum));
                    }
                }
            }
            else
            {
                Debug.Log("found no samples");
            }

            // all done
            Debug.Log("====================> OnComplete : " + complete);
            complete?.Invoke(result, dataType);
        });
    }

    private void ReadCharacteristic(HKDataType dataType, Action<List<string>, HKDataType> complete)
    {
        string typeName = HealthKitDataTypes.GetIdentifier(dataType);
        Debug.LogFormat("reading {0}", typeName);
        List<string> result = new List<string>();
        this.healthStore.ReadCharacteristic(dataType, delegate (Characteristic characteristic, Error error)
        {
            Debug.Log("FINISHED");
            result.Add(string.Format("{0}/{1}", dataType, characteristic));

            // all done
            Debug.Log("====================> OnComplete : " + complete);
            complete?.Invoke(result, dataType);
        });

    }
    private void ReadCorrelationData(HKDataType dataType, DateTimeOffset start, DateTimeOffset end, Action<List<string>, HKDataType> complete)
    {
        List<string> result = new List<string>();
        this.healthStore.ReadCorrelationSamples(dataType, start, end, delegate (List<CorrelationSample> samples, Error error)
        {
            foreach (CorrelationSample correlation in samples)
            {
                foreach (Sample sample in correlation.objects)
                {
                    QuantitySample s = (QuantitySample)sample;
                    result.Add(string.Format("{0}/{1}", s.quantityType, s.quantity.doubleValue));
                }
            }
            // all done
            Debug.Log("====================> OnComplete : " + complete);
            complete?.Invoke(result, dataType);
        });
    }

    private void ReadWorkoutData(HKDataType dataType, DateTimeOffset start, DateTimeOffset end, Action<List<string>, HKDataType> complete)
    {
        List<string> result = new List<string>();
        this.healthStore.ReadWorkoutSamples(WorkoutActivityType.FitnessGaming, start, end, delegate (List<WorkoutSample> samples, Error error)
        {
            foreach (WorkoutSample sample in samples)
            {
                result.Add(sample.ToString());
            }

            // all done
            Debug.Log("====================> OnComplete : " + complete);
            complete?.Invoke(result, dataType);
        });
    }

    public override bool IsAuthorized()
    {
        return isAuthorized;
    }

    public override void SetAuthorized(bool isOn)
    {
        isAuthorized = isOn;
    }
    public string GetDataTitleName(HKDataType hKDataType)
    {
        switch (hKDataType)
        {
            case HKDataType.HKQuantityTypeIdentifierStepCount: return "Steps";
            case HKDataType.HKQuantityTypeIdentifierAppleExerciseTime: return "Exercise Time";
            default: return GetHKDataTypeName(hKDataType);
        }
    }
}
