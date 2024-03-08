﻿using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

[BsonKnownTypes(typeof(WeaponInfo),typeof(ArmorInfo))]
[CreateAssetMenu(fileName = "NewItemInfo", menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameObject Prefab;

    public virtual ItemType ItemType => ItemType.Other;
}