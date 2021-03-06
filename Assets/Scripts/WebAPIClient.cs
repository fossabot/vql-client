﻿//------------------------------------------------------------------------------
// <copyright file="WebAPIClient.cs" company="FurtherSystem Co.,Ltd.">
// Copyright (C) 2020 FurtherSystem Co.,Ltd.
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
// </copyright>
// <author>FurtherSystem Co.,Ltd.</author>
// <email>info@furthersystem.com</email>
// <summary>
// vQL WebAPI Client
// </summary>
//------------------------------------------------------------------------------
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
namespace Com.FurtherSystems.vQL.Client {

    public class WebAPIClient : MonoBehaviour
    {
        const string Url = "http://192.168.1.30:7000";
        const string UserAgent = "vQLClient Unity";
        const string ClientVersion = "v1.0.0";

        const string traditionalKey = "KIWIKIWIKIWIKIWIKIWIKIWIKIWIKIWI";

        [Serializable]
        struct ReqBodyCreate
        {
            public string UnsafeAuthId;
            public string Uuid;
            public string PlatformInfo;
            public long Ticks;
        }

        private string Encode(object obj)
        {
            var postDataJsonBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(obj));
            //Debug.Log("send json bytes: " + BitConverter.ToString(postDataJsonBytes));
            //var encoded = new BouncyCastleWrapper(new AesEngine(), new Pkcs7Padding()).Encrypt(postDataJsonBytes, traditionalKey);
            //Debug.Log("send encrypted bytes: " + BitConverter.ToString(encoded));
            //Debug.Log("send base64 bytes: " + BitConverter.ToString(Encoding.UTF8.GetBytes(Convert.ToBase64String(encoded))));
            return Convert.ToBase64String(postDataJsonBytes);
        }

        public bool CreateResult { get; set; }
        public IEnumerator Create(string name, string caption, string oauth)
        {
            Debug.Log("Create start");
            ReqBodyCreate reqBody;
            reqBody.UnsafeAuthId = "aaa";
            reqBody.Uuid = SystemInfo.deviceUniqueIdentifier;
            reqBody.PlatformInfo = SystemInfo.operatingSystem;
            reqBody.Ticks = ToUnixTime(DateTime.UtcNow);

            UnityWebRequest req = UnityWebRequest.Post(Url + "/vendor/new", Encode(reqBody));
            req.SetRequestHeader("User-Agent", UserAgent + " " + ClientVersion);
            yield return req.SendWebRequest();
            if (req.isNetworkError)
            {
                Debug.LogError(req.error);
                CreateResult = false;
                yield break;
            }
            var res = req.downloadHandler.text;
            if (req.responseCode != 200)
            {
                Debug.Log("http status code:" + req.responseCode);
                CreateResult = false;
                yield break;
            }
            
            Debug.Log("get version end" + res);
            CreateResult = true;
        }

        public static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static long ToUnixTime(DateTime targetTime)
        {
            targetTime = targetTime.ToUniversalTime();
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}