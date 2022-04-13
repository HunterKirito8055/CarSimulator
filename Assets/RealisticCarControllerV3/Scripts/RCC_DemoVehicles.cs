//----------------------------------------------
// Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCC_DemoVehicles : ScriptableObject
{

    public VehicleInfo[] vehiclesNormal;
    public VehicleInfo[] towVehicles;
    public VehicleInfo[] busVehicles;

    #region singleton
    private static RCC_DemoVehicles instance;
    public static RCC_DemoVehicles Instance { get { if (instance == null) instance = Resources.Load("RCC Assets/RCC_DemoVehicles") as RCC_DemoVehicles; return instance; } }
    #endregion

}

[System.Serializable]
public class VehicleInfo
{
    public string vehicleName;
    public RCC_CarControllerV3 RCC_carController;
    public int vehiclePrice;
}

public enum VehicleMode
{
    Normalvehicle,
    Bus,
    TowVehicle,
    Tutorial
}