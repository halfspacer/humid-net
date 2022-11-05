using System;
using System.Runtime.InteropServices;
using App.Scripts.Netcode.Base;
using App.Scripts.Netcode.Helpers;
using App.Scripts.Netcode.Interfaces;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;
using UnityEngine;
using Credentials = Epic.OnlineServices.Connect.Credentials;
using LoginOptions = Epic.OnlineServices.Connect.LoginOptions;

namespace App.Scripts.Netcode.EOS {
    public class EOSSDK : NetworkManager, ITick, IAuthenticate, IInitialize, IUninitialize {
        private const float CPlatformTickInterval = 0.1f;

        private static PlatformInterface _platformInterface;

        // Set these values as appropriate. For more information, see the Developer Portal documentation.
        [SerializeField] private string productName = "MyUnityApplication";
        [SerializeField] private string productVersion = "1.0";
        [SerializeField] private string productId = "";
        [SerializeField] private string sandboxId = "";
        [SerializeField] private string deploymentId = "";
        [SerializeField] private string clientId = "";
        [SerializeField] private string clientSecret = "";
        private float _platformTickTimer;

        // If we're in editor, we should dynamically load and unload the SDK between play sessions.
        // This allows us to initialize the SDK each time the game is run in editor.
#if UNITY_EDITOR
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("Kernel32.dll")]
        private static extern int FreeLibrary(IntPtr hLibModule);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        private IntPtr _libraryPointer;
#endif
        public void Tick() {
            if (_platformInterface != null) {
                _platformTickTimer += Time.deltaTime;

                if (_platformTickTimer >= CPlatformTickInterval) {
                    _platformTickTimer = 0;
                    _platformInterface.Tick();
                }
            }
        }

        public void Authenticate(Action<ResultData> callback) {
            var deviceID = SystemInfo.deviceUniqueIdentifier;
            
            //Limit the length of the device ID to 32 characters or less
            if (deviceID.Length > 32) {
                deviceID = deviceID[..32];
            }
            
            var loginOptions = new LoginOptions {
                Credentials = new Credentials {
                    Token = null,
                    Type = ExternalCredentialType.DeviceidAccessToken
                },
                UserLoginInfo = new UserLoginInfo {
                    DisplayName = deviceID
                }
            };

            var createDeviceIdOptions = new CreateDeviceIdOptions {
                DeviceModel = SystemInfo.deviceType.ToString(),
            };
            
            _platformInterface.GetConnectInterface().CreateDeviceId(ref createDeviceIdOptions, null, (ref CreateDeviceIdCallbackInfo info) => {
                if (info.ResultCode == Result.DuplicateNotAllowed) {
                    Debug.Log("Device ID already exists");
                }
                else if (info.ResultCode == Result.Success) {
                    Debug.Log("Successfully created device id");
                }
                else {
                    callback?.Invoke(new ResultData {
                        result = Results.Failure,
                        message = "Failed to create device id"
                    });
                    return;
                }
                
                _platformInterface.GetConnectInterface().Login(ref loginOptions, null,
                    (ref Epic.OnlineServices.Connect.LoginCallbackInfo data) => {
                        if (data.ResultCode == Result.Success) {
                            callback?.Invoke(new ResultData {
                                result = Results.Success,
                                message = "Login success"
                            });
                        }
                        else {
                            callback?.Invoke(new ResultData {
                                result = Results.Failure,
                                message = "Login failed"
                            });
                        }
                    });
            });
        }

        public void Initialize(Action<ResultData> onComplete) {
#if UNITY_EDITOR
            const string libraryPath = "Assets/Plugins/EOS/Bin/" + Config.LibraryName;

            _libraryPointer = LoadLibrary(libraryPath);
            if (_libraryPointer == IntPtr.Zero) {
                throw new Exception("Failed to load library" + libraryPath);
            }

            Bindings.Hook(_libraryPointer, GetProcAddress);
#endif
            
            var initializeOptions = new InitializeOptions {
                ProductName = productName,
                ProductVersion = productVersion
            };

            var initializeResult = PlatformInterface.Initialize(ref initializeOptions);
            if (initializeResult != Result.Success) {
                onComplete?.Invoke(new ResultData {
                    result = Results.Failure,
                    message = "Failed to initialize platform: " + initializeResult
                });
            }

            // The SDK outputs lots of information that is useful for debugging.
            // Make sure to set up the logging interface as early as possible: after initializing.
            LoggingInterface.SetLogLevel(LogCategory.AllCategories, LogLevel.Warning);
            LoggingInterface.SetCallback((ref LogMessage logMessage) => Debug.Log(logMessage.Message));

            var options = new Options {
                ProductId = productId,
                SandboxId = sandboxId,
                DeploymentId = deploymentId,
                ClientCredentials = new ClientCredentials {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            };

            _platformInterface = PlatformInterface.Create(ref options);
            if (_platformInterface == null) {
                onComplete?.Invoke(new ResultData {
                    result = Results.Failure,
                    message = "Failed to create platform"
                });
            } else {
                onComplete?.Invoke(new ResultData {
                    result = Results.Success,
                    message = "Platform created"
                });
            }
        }

        public void Uninitialize(Action<ResultData> onComplete = null) {
            if (_platformInterface != null) {
                _platformInterface.Release();
                _platformInterface = null;
                PlatformInterface.Shutdown();
            }

#if UNITY_EDITOR
            if (_libraryPointer == IntPtr.Zero) {
                return;
            }

            Bindings.Unhook();

            // Free until the module ref count is 0
            while (FreeLibrary(_libraryPointer) != 0) {
            }

            _libraryPointer = IntPtr.Zero;
#endif
        }
    }
}