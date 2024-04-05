using System;
using System.Collections.Generic;
using UnityEngine;

namespace SheetCodes
{
	//Generated code, do not edit!

	public static class ModelManager
	{
        private static Dictionary<DatasheetType, LoadRequest> loadRequests;

        static ModelManager()
        {
            loadRequests = new Dictionary<DatasheetType, LoadRequest>();
        }

        public static void InitializeAll()
        {
            DatasheetType[] values = Enum.GetValues(typeof(DatasheetType)) as DatasheetType[];
            foreach(DatasheetType value in values)
                Initialize(value);
        }
		
        public static void Unload(DatasheetType datasheetType)
        {
            switch (datasheetType)
            {
                case DatasheetType.Particle:
                    {
                        if (particleModel == null || particleModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(particleModel);
                        particleModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Particle, out request))
                        {
                            loadRequests.Remove(DatasheetType.Particle);
                            request.resourceRequest.completed -= OnLoadCompleted_ParticleModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Projectile:
                    {
                        if (projectileModel == null || projectileModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(projectileModel);
                        projectileModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Projectile, out request))
                        {
                            loadRequests.Remove(DatasheetType.Projectile);
                            request.resourceRequest.completed -= OnLoadCompleted_ProjectileModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.AiShip:
                    {
                        if (aiShipModel == null || aiShipModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(aiShipModel);
                        aiShipModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.AiShip, out request))
                        {
                            loadRequests.Remove(DatasheetType.AiShip);
                            request.resourceRequest.completed -= OnLoadCompleted_AiShipModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Scene:
                    {
                        if (sceneModel == null || sceneModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(sceneModel);
                        sceneModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Scene, out request))
                        {
                            loadRequests.Remove(DatasheetType.Scene);
                            request.resourceRequest.completed -= OnLoadCompleted_SceneModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.PopupMessage:
                    {
                        if (popupMessageModel == null || popupMessageModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(popupMessageModel);
                        popupMessageModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.PopupMessage, out request))
                        {
                            loadRequests.Remove(DatasheetType.PopupMessage);
                            request.resourceRequest.completed -= OnLoadCompleted_PopupMessageModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.AiWeapon:
                    {
                        if (aiWeaponModel == null || aiWeaponModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(aiWeaponModel);
                        aiWeaponModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.AiWeapon, out request))
                        {
                            loadRequests.Remove(DatasheetType.AiWeapon);
                            request.resourceRequest.completed -= OnLoadCompleted_AiWeaponModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.PlayerShip:
                    {
                        if (playerShipModel == null || playerShipModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(playerShipModel);
                        playerShipModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.PlayerShip, out request))
                        {
                            loadRequests.Remove(DatasheetType.PlayerShip);
                            request.resourceRequest.completed -= OnLoadCompleted_PlayerShipModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.PlayerWeapon:
                    {
                        if (playerWeaponModel == null || playerWeaponModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(playerWeaponModel);
                        playerWeaponModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.PlayerWeapon, out request))
                        {
                            loadRequests.Remove(DatasheetType.PlayerWeapon);
                            request.resourceRequest.completed -= OnLoadCompleted_PlayerWeaponModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Environment:
                    {
                        if (environmentModel == null || environmentModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(environmentModel);
                        environmentModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Environment, out request))
                        {
                            loadRequests.Remove(DatasheetType.Environment);
                            request.resourceRequest.completed -= OnLoadCompleted_EnvironmentModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                case DatasheetType.Audio:
                    {
                        if (audioModel == null || audioModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to unload model {0}. Model is not loaded.", datasheetType));
                            break;
                        }
                        Resources.UnloadAsset(audioModel);
                        audioModel = null;
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Audio, out request))
                        {
                            loadRequests.Remove(DatasheetType.Audio);
                            request.resourceRequest.completed -= OnLoadCompleted_AudioModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(false);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public static void Initialize(DatasheetType datasheetType)
        {
            switch (datasheetType)
            {
                case DatasheetType.Particle:
                    {
                        if (particleModel != null && !particleModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        particleModel = Resources.Load<ParticleModel>("ScriptableObjects/Particle");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Particle, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Particle);
                            request.resourceRequest.completed -= OnLoadCompleted_ParticleModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Projectile:
                    {
                        if (projectileModel != null && !projectileModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        projectileModel = Resources.Load<ProjectileModel>("ScriptableObjects/Projectile");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Projectile, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Projectile);
                            request.resourceRequest.completed -= OnLoadCompleted_ProjectileModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.AiShip:
                    {
                        if (aiShipModel != null && !aiShipModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        aiShipModel = Resources.Load<AiShipModel>("ScriptableObjects/AiShip");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.AiShip, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.AiShip);
                            request.resourceRequest.completed -= OnLoadCompleted_AiShipModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Scene:
                    {
                        if (sceneModel != null && !sceneModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        sceneModel = Resources.Load<SceneModel>("ScriptableObjects/Scene");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Scene, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Scene);
                            request.resourceRequest.completed -= OnLoadCompleted_SceneModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.PopupMessage:
                    {
                        if (popupMessageModel != null && !popupMessageModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        popupMessageModel = Resources.Load<PopupMessageModel>("ScriptableObjects/PopupMessage");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.PopupMessage, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.PopupMessage);
                            request.resourceRequest.completed -= OnLoadCompleted_PopupMessageModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.AiWeapon:
                    {
                        if (aiWeaponModel != null && !aiWeaponModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        aiWeaponModel = Resources.Load<AiWeaponModel>("ScriptableObjects/AiWeapon");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.AiWeapon, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.AiWeapon);
                            request.resourceRequest.completed -= OnLoadCompleted_AiWeaponModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.PlayerShip:
                    {
                        if (playerShipModel != null && !playerShipModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        playerShipModel = Resources.Load<PlayerShipModel>("ScriptableObjects/PlayerShip");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.PlayerShip, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.PlayerShip);
                            request.resourceRequest.completed -= OnLoadCompleted_PlayerShipModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.PlayerWeapon:
                    {
                        if (playerWeaponModel != null && !playerWeaponModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        playerWeaponModel = Resources.Load<PlayerWeaponModel>("ScriptableObjects/PlayerWeapon");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.PlayerWeapon, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.PlayerWeapon);
                            request.resourceRequest.completed -= OnLoadCompleted_PlayerWeaponModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Environment:
                    {
                        if (environmentModel != null && !environmentModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        environmentModel = Resources.Load<EnvironmentModel>("ScriptableObjects/Environment");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Environment, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Environment);
                            request.resourceRequest.completed -= OnLoadCompleted_EnvironmentModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                case DatasheetType.Audio:
                    {
                        if (audioModel != null && !audioModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to Initialize {0}. Model is already initialized.", datasheetType));
                            break;
                        }

                        audioModel = Resources.Load<AudioModel>("ScriptableObjects/Audio");
                        LoadRequest request;
                        if (loadRequests.TryGetValue(DatasheetType.Audio, out request))
                        {
                            Log(string.Format("Sheet Codes: Trying to initialize {0} while also async loading. Async load has been canceled.", datasheetType));
                            loadRequests.Remove(DatasheetType.Audio);
                            request.resourceRequest.completed -= OnLoadCompleted_AudioModel;
							foreach (Action<bool> callback in request.callbacks)
								callback(true);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public static void InitializeAsync(DatasheetType datasheetType, Action<bool> callback)
        {
            switch (datasheetType)
            {
                case DatasheetType.Particle:
                    {
                        if (particleModel != null && !particleModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Particle))
                        {
                            loadRequests[DatasheetType.Particle].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<ParticleModel>("ScriptableObjects/Particle");
                        loadRequests.Add(DatasheetType.Particle, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_ParticleModel;
                        break;
                    }
                case DatasheetType.Projectile:
                    {
                        if (projectileModel != null && !projectileModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Projectile))
                        {
                            loadRequests[DatasheetType.Projectile].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<ProjectileModel>("ScriptableObjects/Projectile");
                        loadRequests.Add(DatasheetType.Projectile, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_ProjectileModel;
                        break;
                    }
                case DatasheetType.AiShip:
                    {
                        if (aiShipModel != null && !aiShipModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.AiShip))
                        {
                            loadRequests[DatasheetType.AiShip].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<AiShipModel>("ScriptableObjects/AiShip");
                        loadRequests.Add(DatasheetType.AiShip, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_AiShipModel;
                        break;
                    }
                case DatasheetType.Scene:
                    {
                        if (sceneModel != null && !sceneModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Scene))
                        {
                            loadRequests[DatasheetType.Scene].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<SceneModel>("ScriptableObjects/Scene");
                        loadRequests.Add(DatasheetType.Scene, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_SceneModel;
                        break;
                    }
                case DatasheetType.PopupMessage:
                    {
                        if (popupMessageModel != null && !popupMessageModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.PopupMessage))
                        {
                            loadRequests[DatasheetType.PopupMessage].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<PopupMessageModel>("ScriptableObjects/PopupMessage");
                        loadRequests.Add(DatasheetType.PopupMessage, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_PopupMessageModel;
                        break;
                    }
                case DatasheetType.AiWeapon:
                    {
                        if (aiWeaponModel != null && !aiWeaponModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.AiWeapon))
                        {
                            loadRequests[DatasheetType.AiWeapon].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<AiWeaponModel>("ScriptableObjects/AiWeapon");
                        loadRequests.Add(DatasheetType.AiWeapon, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_AiWeaponModel;
                        break;
                    }
                case DatasheetType.PlayerShip:
                    {
                        if (playerShipModel != null && !playerShipModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.PlayerShip))
                        {
                            loadRequests[DatasheetType.PlayerShip].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<PlayerShipModel>("ScriptableObjects/PlayerShip");
                        loadRequests.Add(DatasheetType.PlayerShip, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_PlayerShipModel;
                        break;
                    }
                case DatasheetType.PlayerWeapon:
                    {
                        if (playerWeaponModel != null && !playerWeaponModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.PlayerWeapon))
                        {
                            loadRequests[DatasheetType.PlayerWeapon].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<PlayerWeaponModel>("ScriptableObjects/PlayerWeapon");
                        loadRequests.Add(DatasheetType.PlayerWeapon, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_PlayerWeaponModel;
                        break;
                    }
                case DatasheetType.Environment:
                    {
                        if (environmentModel != null && !environmentModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Environment))
                        {
                            loadRequests[DatasheetType.Environment].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<EnvironmentModel>("ScriptableObjects/Environment");
                        loadRequests.Add(DatasheetType.Environment, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_EnvironmentModel;
                        break;
                    }
                case DatasheetType.Audio:
                    {
                        if (audioModel != null && !audioModel.Equals(null))
                        {
                            Log(string.Format("Sheet Codes: Trying to InitializeAsync {0}. Model is already initialized.", datasheetType));
                            callback(true);
                            break;
                        }
                        if(loadRequests.ContainsKey(DatasheetType.Audio))
                        {
                            loadRequests[DatasheetType.Audio].callbacks.Add(callback);
                            break;
                        }
                        ResourceRequest request = Resources.LoadAsync<AudioModel>("ScriptableObjects/Audio");
                        loadRequests.Add(DatasheetType.Audio, new LoadRequest(request, callback));
                        request.completed += OnLoadCompleted_AudioModel;
                        break;
                    }
                default:
                    break;
            }
        }

        private static void OnLoadCompleted_ParticleModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Particle];
            particleModel = request.resourceRequest.asset as ParticleModel;
            loadRequests.Remove(DatasheetType.Particle);
            operation.completed -= OnLoadCompleted_ParticleModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static ParticleModel particleModel = default;
		public static ParticleModel ParticleModel
        {
            get
            {
                if (particleModel == null)
                    Initialize(DatasheetType.Particle);

                return particleModel;
            }
        }
        private static void OnLoadCompleted_ProjectileModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Projectile];
            projectileModel = request.resourceRequest.asset as ProjectileModel;
            loadRequests.Remove(DatasheetType.Projectile);
            operation.completed -= OnLoadCompleted_ProjectileModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static ProjectileModel projectileModel = default;
		public static ProjectileModel ProjectileModel
        {
            get
            {
                if (projectileModel == null)
                    Initialize(DatasheetType.Projectile);

                return projectileModel;
            }
        }
        private static void OnLoadCompleted_AiShipModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.AiShip];
            aiShipModel = request.resourceRequest.asset as AiShipModel;
            loadRequests.Remove(DatasheetType.AiShip);
            operation.completed -= OnLoadCompleted_AiShipModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static AiShipModel aiShipModel = default;
		public static AiShipModel AiShipModel
        {
            get
            {
                if (aiShipModel == null)
                    Initialize(DatasheetType.AiShip);

                return aiShipModel;
            }
        }
        private static void OnLoadCompleted_SceneModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Scene];
            sceneModel = request.resourceRequest.asset as SceneModel;
            loadRequests.Remove(DatasheetType.Scene);
            operation.completed -= OnLoadCompleted_SceneModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static SceneModel sceneModel = default;
		public static SceneModel SceneModel
        {
            get
            {
                if (sceneModel == null)
                    Initialize(DatasheetType.Scene);

                return sceneModel;
            }
        }
        private static void OnLoadCompleted_PopupMessageModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.PopupMessage];
            popupMessageModel = request.resourceRequest.asset as PopupMessageModel;
            loadRequests.Remove(DatasheetType.PopupMessage);
            operation.completed -= OnLoadCompleted_PopupMessageModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static PopupMessageModel popupMessageModel = default;
		public static PopupMessageModel PopupMessageModel
        {
            get
            {
                if (popupMessageModel == null)
                    Initialize(DatasheetType.PopupMessage);

                return popupMessageModel;
            }
        }
        private static void OnLoadCompleted_AiWeaponModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.AiWeapon];
            aiWeaponModel = request.resourceRequest.asset as AiWeaponModel;
            loadRequests.Remove(DatasheetType.AiWeapon);
            operation.completed -= OnLoadCompleted_AiWeaponModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static AiWeaponModel aiWeaponModel = default;
		public static AiWeaponModel AiWeaponModel
        {
            get
            {
                if (aiWeaponModel == null)
                    Initialize(DatasheetType.AiWeapon);

                return aiWeaponModel;
            }
        }
        private static void OnLoadCompleted_PlayerShipModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.PlayerShip];
            playerShipModel = request.resourceRequest.asset as PlayerShipModel;
            loadRequests.Remove(DatasheetType.PlayerShip);
            operation.completed -= OnLoadCompleted_PlayerShipModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static PlayerShipModel playerShipModel = default;
		public static PlayerShipModel PlayerShipModel
        {
            get
            {
                if (playerShipModel == null)
                    Initialize(DatasheetType.PlayerShip);

                return playerShipModel;
            }
        }
        private static void OnLoadCompleted_PlayerWeaponModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.PlayerWeapon];
            playerWeaponModel = request.resourceRequest.asset as PlayerWeaponModel;
            loadRequests.Remove(DatasheetType.PlayerWeapon);
            operation.completed -= OnLoadCompleted_PlayerWeaponModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static PlayerWeaponModel playerWeaponModel = default;
		public static PlayerWeaponModel PlayerWeaponModel
        {
            get
            {
                if (playerWeaponModel == null)
                    Initialize(DatasheetType.PlayerWeapon);

                return playerWeaponModel;
            }
        }
        private static void OnLoadCompleted_EnvironmentModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Environment];
            environmentModel = request.resourceRequest.asset as EnvironmentModel;
            loadRequests.Remove(DatasheetType.Environment);
            operation.completed -= OnLoadCompleted_EnvironmentModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static EnvironmentModel environmentModel = default;
		public static EnvironmentModel EnvironmentModel
        {
            get
            {
                if (environmentModel == null)
                    Initialize(DatasheetType.Environment);

                return environmentModel;
            }
        }
        private static void OnLoadCompleted_AudioModel(AsyncOperation operation)
        {
            LoadRequest request = loadRequests[DatasheetType.Audio];
            audioModel = request.resourceRequest.asset as AudioModel;
            loadRequests.Remove(DatasheetType.Audio);
            operation.completed -= OnLoadCompleted_AudioModel;
            foreach (Action<bool> callback in request.callbacks)
                callback(true);
        }

		private static AudioModel audioModel = default;
		public static AudioModel AudioModel
        {
            get
            {
                if (audioModel == null)
                    Initialize(DatasheetType.Audio);

                return audioModel;
            }
        }
		
        private static void Log(string message)
        {
            Debug.LogWarning(message);
        }
	}
	
    public struct LoadRequest
    {
        public readonly ResourceRequest resourceRequest;
        public readonly List<Action<bool>> callbacks;

        public LoadRequest(ResourceRequest resourceRequest, Action<bool> callback)
        {
            this.resourceRequest = resourceRequest;
            callbacks = new List<Action<bool>>() { callback };
        }
    }
}
