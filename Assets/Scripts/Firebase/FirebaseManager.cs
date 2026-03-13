using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    
    // === CONFIGURATION FIREBASE ===
    [Header("Configuration Firebase")]
    [Tooltip("URL de ta base Firebase (ex: https://ton-projet.firebaseio.com)")]
    public string firebaseDatabaseURL = "https://TON-PROJET-ID.firebaseio.com";
    
    // === ÉTAT ===
    public bool IsInitialized { get; private set; } = false;
    public bool IsConnected { get; private set; } = false;
    
    // === ÉVÉNEMENTS ===
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string> OnError;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        // Test la connexion au démarrage
        StartCoroutine(TestConnection());
    }
    
    public void Initialize(string databaseURL)
    {
        firebaseDatabaseURL = databaseURL;
        StartCoroutine(TestConnection());
    }
    
    IEnumerator TestConnection()
    {
        if (string.IsNullOrEmpty(firebaseDatabaseURL) || firebaseDatabaseURL.Contains("TON-PROJET"))
        {
            Debug.LogWarning("Firebase: Configure ton URL dans FirebaseManager !");
            IsInitialized = false;
            yield break;
        }
        
        string testURL = firebaseDatabaseURL + "/.json";
        
        using (UnityWebRequest request = UnityWebRequest.Get(testURL))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                IsInitialized = true;
                IsConnected = true;
                Debug.Log("Firebase connecté avec succès !");
                OnConnected?.Invoke();
            }
            else
            {
                IsInitialized = false;
                IsConnected = false;
                Debug.LogError("Firebase: Impossible de se connecter - " + request.error);
                OnError?.Invoke(request.error);
            }
        }
    }
    
    // ========================================
    //          MÉTHODES PRINCIPALES
    // ========================================
    
    public void SaveData(string path, string jsonData, Action<bool> callback = null)
    {
        StartCoroutine(SaveDataCoroutine(path, jsonData, callback));
    }
    
    IEnumerator SaveDataCoroutine(string path, string jsonData, Action<bool> callback)
    {
        string url = firebaseDatabaseURL + "/" + path + ".json";
        
        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            bool success = request.result == UnityWebRequest.Result.Success;
            
            if (success)
            {
                Debug.Log("Données sauvegardées: " + path);
            }
            else
            {
                Debug.LogError("Erreur sauvegarde: " + request.error);
                OnError?.Invoke(request.error);
            }
            
            callback?.Invoke(success);
        }
    }
    
    public void LoadData(string path, Action<string> callback)
    {
        StartCoroutine(LoadDataCoroutine(path, callback));
    }
    
    IEnumerator LoadDataCoroutine(string path, Action<string> callback)
    {
        string url = firebaseDatabaseURL + "/" + path + ".json";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("Données chargées: " + path);
                callback?.Invoke(json);
            }
            else
            {
                Debug.LogError("Erreur chargement: " + request.error);
                OnError?.Invoke(request.error);
                callback?.Invoke(null);
            }
        }
    }
    
    public void DeleteData(string path, Action<bool> callback = null)
    {
        StartCoroutine(DeleteDataCoroutine(path, callback));
    }
    
    IEnumerator DeleteDataCoroutine(string path, Action<bool> callback)
    {
        string url = firebaseDatabaseURL + "/" + path + ".json";
        
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();
            
            bool success = request.result == UnityWebRequest.Result.Success;
            
            if (success)
            {
                Debug.Log("Données supprimées: " + path);
            }
            else
            {
                Debug.LogError("Erreur suppression: " + request.error);
            }
            
            callback?.Invoke(success);
        }
    }
    
    public bool IsReady()
    {
        return IsInitialized && IsConnected;
    }
}

