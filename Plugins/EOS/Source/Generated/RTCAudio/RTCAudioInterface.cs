// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCAudio
{
	public sealed partial class RTCAudioInterface : Handle
	{
		public RTCAudioInterface()
		{
		}

		public RTCAudioInterface(System.IntPtr innerHandle) : base(innerHandle)
		{
		}

		/// <summary>
		/// The most recent version of the <see cref="AddNotifyAudioBeforeRender" /> API.
		/// </summary>
		public const int AddnotifyaudiobeforerenderApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AddNotifyAudioBeforeSend" /> API.
		/// </summary>
		public const int AddnotifyaudiobeforesendApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AddNotifyAudioDevicesChanged" /> API.
		/// </summary>
		public const int AddnotifyaudiodeviceschangedApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AddNotifyAudioInputState" /> API.
		/// </summary>
		public const int AddnotifyaudioinputstateApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AddNotifyAudioOutputState" /> API.
		/// </summary>
		public const int AddnotifyaudiooutputstateApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AddNotifyParticipantUpdated" /> API.
		/// </summary>
		public const int AddnotifyparticipantupdatedApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AudioBuffer" /> API
		/// </summary>
		public const int AudiobufferApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AudioInputDeviceInfo" /> struct.
		/// </summary>
		public const int AudioinputdeviceinfoApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="AudioOutputDeviceInfo" /> struct.
		/// </summary>
		public const int AudiooutputdeviceinfoApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="GetAudioInputDeviceByIndex" /> API.
		/// </summary>
		public const int GetaudioinputdevicebyindexApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="GetAudioInputDevicesCount" /> API.
		/// </summary>
		public const int GetaudioinputdevicescountApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="GetAudioOutputDeviceByIndex" /> API.
		/// </summary>
		public const int GetaudiooutputdevicebyindexApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="GetAudioOutputDevicesCount" /> API.
		/// </summary>
		public const int GetaudiooutputdevicescountApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="RegisterPlatformAudioUser" /> API.
		/// </summary>
		public const int RegisterplatformaudiouserApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="SendAudio" /> API.
		/// </summary>
		public const int SendaudioApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="SetAudioInputSettings" /> API.
		/// </summary>
		public const int SetaudioinputsettingsApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="SetAudioOutputSettings" /> API.
		/// </summary>
		public const int SetaudiooutputsettingsApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="UnregisterPlatformAudioUser" /> API.
		/// </summary>
		public const int UnregisterplatformaudiouserApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="UpdateParticipantVolume" /> API.
		/// </summary>
		public const int UpdateparticipantvolumeApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="UpdateReceiving" /> API.
		/// </summary>
		public const int UpdatereceivingApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="UpdateReceivingVolume" /> API.
		/// </summary>
		public const int UpdatereceivingvolumeApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="UpdateSending" /> API.
		/// </summary>
		public const int UpdatesendingApiLatest = 1;

		/// <summary>
		/// The most recent version of the <see cref="UpdateSendingVolume" /> API.
		/// </summary>
		public const int UpdatesendingvolumeApiLatest = 1;

		/// <summary>
		/// Register to receive notifications with remote audio buffers before they are rendered.
		/// 
		/// This gives you access to the audio data received, allowing for example the implementation of custom filters/effects.
		/// 
		/// If the returned NotificationId is valid, you must call <see cref="RemoveNotifyAudioBeforeRender" /> when you no longer wish to
		/// have your CompletionDelegate called.
		/// <seealso cref="Common.InvalidNotificationid" />
		/// <seealso cref="RemoveNotifyAudioBeforeRender" />
		/// </summary>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when a presence change occurs</param>
		/// <returns>
		/// Notification ID representing the registered callback if successful, an invalid NotificationId if not
		/// </returns>
		public ulong AddNotifyAudioBeforeRender(ref AddNotifyAudioBeforeRenderOptions options, object clientData, OnAudioBeforeRenderCallback completionDelegate)
		{
			AddNotifyAudioBeforeRenderOptionsInternal optionsInternal = new AddNotifyAudioBeforeRenderOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnAudioBeforeRenderCallbackInternal(OnAudioBeforeRenderCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			var funcResult = Bindings.EOS_RTCAudio_AddNotifyAudioBeforeRender(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);

			Helper.AssignNotificationIdToCallback(clientDataAddress, funcResult);

			return funcResult;
		}

		/// <summary>
		/// Register to receive notifications when local audio buffers are about to be encoded and sent.
		/// 
		/// This gives you access to the audio data about to be sent, allowing for example the implementation of custom filters/effects.
		/// 
		/// If the returned NotificationId is valid, you must call <see cref="RemoveNotifyAudioBeforeSend" /> when you no longer wish to
		/// have your CompletionDelegate called.
		/// <seealso cref="Common.InvalidNotificationid" />
		/// <seealso cref="RemoveNotifyAudioBeforeSend" />
		/// </summary>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when a presence change occurs</param>
		/// <returns>
		/// Notification ID representing the registered callback if successful, an invalid NotificationId if not
		/// </returns>
		public ulong AddNotifyAudioBeforeSend(ref AddNotifyAudioBeforeSendOptions options, object clientData, OnAudioBeforeSendCallback completionDelegate)
		{
			AddNotifyAudioBeforeSendOptionsInternal optionsInternal = new AddNotifyAudioBeforeSendOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnAudioBeforeSendCallbackInternal(OnAudioBeforeSendCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			var funcResult = Bindings.EOS_RTCAudio_AddNotifyAudioBeforeSend(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);

			Helper.AssignNotificationIdToCallback(clientDataAddress, funcResult);

			return funcResult;
		}

		/// <summary>
		/// Register to receive notifications when an audio device is added or removed to the system.
		/// 
		/// If the returned NotificationId is valid, you must call <see cref="RemoveNotifyAudioDevicesChanged" /> when you no longer wish
		/// to have your CompletionDelegate called.
		/// 
		/// The library will try to use user selected audio device while following these rules:
		/// - if none of the audio devices has been available and connected before - the library will try to use it;
		/// - if user selected device failed for some reason, default device will be used instead (and user selected device will be memorized);
		/// - if user selected a device but it was not used for some reason (and default was used instead), when devices selection is triggered we will try to use user selected device again;
		/// - triggers to change a device: when new audio device appears or disappears - library will try to use previously user selected;
		/// - if for any reason, a device cannot be used - the library will fallback to using default;
		/// - if a configuration of the current audio device has been changed, it will be restarted.
		/// <seealso cref="Common.InvalidNotificationid" />
		/// <seealso cref="RemoveNotifyAudioDevicesChanged" />
		/// <seealso cref="SetAudioInputSettingsOptions" />
		/// <seealso cref="SetAudioOutputSettingsOptions" />
		/// </summary>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when an audio device change occurs</param>
		/// <returns>
		/// Notification ID representing the registered callback if successful, an invalid NotificationId if not
		/// </returns>
		public ulong AddNotifyAudioDevicesChanged(ref AddNotifyAudioDevicesChangedOptions options, object clientData, OnAudioDevicesChangedCallback completionDelegate)
		{
			AddNotifyAudioDevicesChangedOptionsInternal optionsInternal = new AddNotifyAudioDevicesChangedOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnAudioDevicesChangedCallbackInternal(OnAudioDevicesChangedCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			var funcResult = Bindings.EOS_RTCAudio_AddNotifyAudioDevicesChanged(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);

			Helper.AssignNotificationIdToCallback(clientDataAddress, funcResult);

			return funcResult;
		}

		/// <summary>
		/// Register to receive notifications when audio input state changed.
		/// 
		/// If the returned NotificationId is valid, you must call <see cref="RemoveNotifyAudioInputState" /> when you no longer wish to
		/// have your CompletionDelegate called.
		/// <seealso cref="Common.InvalidNotificationid" />
		/// <seealso cref="RemoveNotifyAudioInputState" />
		/// </summary>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when audio input state changes</param>
		/// <returns>
		/// Notification ID representing the registered callback if successful, an invalid NotificationId if not
		/// </returns>
		public ulong AddNotifyAudioInputState(ref AddNotifyAudioInputStateOptions options, object clientData, OnAudioInputStateCallback completionDelegate)
		{
			AddNotifyAudioInputStateOptionsInternal optionsInternal = new AddNotifyAudioInputStateOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnAudioInputStateCallbackInternal(OnAudioInputStateCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			var funcResult = Bindings.EOS_RTCAudio_AddNotifyAudioInputState(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);

			Helper.AssignNotificationIdToCallback(clientDataAddress, funcResult);

			return funcResult;
		}

		/// <summary>
		/// Register to receive notifications when audio output state changed.
		/// 
		/// If the returned NotificationId is valid, you must call <see cref="RemoveNotifyAudioOutputState" /> when you no longer wish to
		/// have your CompletionDelegate called.
		/// <seealso cref="Common.InvalidNotificationid" />
		/// <seealso cref="RemoveNotifyAudioOutputState" />
		/// </summary>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when audio output state changes</param>
		/// <returns>
		/// Notification ID representing the registered callback if successful, an invalid NotificationId if not
		/// </returns>
		public ulong AddNotifyAudioOutputState(ref AddNotifyAudioOutputStateOptions options, object clientData, OnAudioOutputStateCallback completionDelegate)
		{
			AddNotifyAudioOutputStateOptionsInternal optionsInternal = new AddNotifyAudioOutputStateOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnAudioOutputStateCallbackInternal(OnAudioOutputStateCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			var funcResult = Bindings.EOS_RTCAudio_AddNotifyAudioOutputState(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);

			Helper.AssignNotificationIdToCallback(clientDataAddress, funcResult);

			return funcResult;
		}

		/// <summary>
		/// Register to receive notifications when a room participant audio status is updated (f.e when speaking flag changes).
		/// 
		/// If the returned NotificationId is valid, you must call <see cref="RemoveNotifyParticipantUpdated" /> when you no longer wish
		/// to have your CompletionDelegate called.
		/// <seealso cref="Common.InvalidNotificationid" />
		/// <seealso cref="RemoveNotifyParticipantUpdated" />
		/// </summary>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when a presence change occurs</param>
		/// <returns>
		/// Notification ID representing the registered callback if successful, an invalid NotificationId if not
		/// </returns>
		public ulong AddNotifyParticipantUpdated(ref AddNotifyParticipantUpdatedOptions options, object clientData, OnParticipantUpdatedCallback completionDelegate)
		{
			AddNotifyParticipantUpdatedOptionsInternal optionsInternal = new AddNotifyParticipantUpdatedOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnParticipantUpdatedCallbackInternal(OnParticipantUpdatedCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			var funcResult = Bindings.EOS_RTCAudio_AddNotifyParticipantUpdated(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);

			Helper.AssignNotificationIdToCallback(clientDataAddress, funcResult);

			return funcResult;
		}

		/// <summary>
		/// Fetches an audio input device's info from then given index. The returned value should not be cached and important
		/// information should be copied off of the result object immediately.
		/// <seealso cref="GetAudioInputDevicesCount" />
		/// <seealso cref="AddNotifyAudioDevicesChanged" />
		/// </summary>
		/// <param name="options">structure containing the index being accessed</param>
		/// <returns>
		/// A pointer to the device information, or <see langword="null" /> on error. You should NOT keep hold of this pointer.
		/// </returns>
		public AudioInputDeviceInfo? GetAudioInputDeviceByIndex(ref GetAudioInputDeviceByIndexOptions options)
		{
			GetAudioInputDeviceByIndexOptionsInternal optionsInternal = new GetAudioInputDeviceByIndexOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_GetAudioInputDeviceByIndex(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			AudioInputDeviceInfo? funcResultReturn;
			Helper.Get<AudioInputDeviceInfoInternal, AudioInputDeviceInfo>(funcResult, out funcResultReturn);
			return funcResultReturn;
		}

		/// <summary>
		/// Returns the number of audio input devices available in the system.
		/// 
		/// The returned value should not be cached and should instead be used immediately with the <see cref="GetAudioInputDeviceByIndex" />
		/// function.
		/// <seealso cref="GetAudioInputDeviceByIndex" />
		/// <seealso cref="AddNotifyAudioDevicesChanged" />
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation</param>
		/// <returns>
		/// The number of audio input devices
		/// </returns>
		public uint GetAudioInputDevicesCount(ref GetAudioInputDevicesCountOptions options)
		{
			GetAudioInputDevicesCountOptionsInternal optionsInternal = new GetAudioInputDevicesCountOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_GetAudioInputDevicesCount(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			return funcResult;
		}

		/// <summary>
		/// Fetches an audio output device's info from then given index.
		/// 
		/// The returned value should not be cached and important information should be copied off of the result object immediately.
		/// <seealso cref="GetAudioOutputDevicesCount" />
		/// <seealso cref="AddNotifyAudioDevicesChanged" />
		/// </summary>
		/// <param name="options">structure containing the index being accessed</param>
		/// <returns>
		/// A pointer to the device information, or <see langword="null" /> on error. You should NOT keep hold of this pointer.
		/// </returns>
		public AudioOutputDeviceInfo? GetAudioOutputDeviceByIndex(ref GetAudioOutputDeviceByIndexOptions options)
		{
			GetAudioOutputDeviceByIndexOptionsInternal optionsInternal = new GetAudioOutputDeviceByIndexOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_GetAudioOutputDeviceByIndex(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			AudioOutputDeviceInfo? funcResultReturn;
			Helper.Get<AudioOutputDeviceInfoInternal, AudioOutputDeviceInfo>(funcResult, out funcResultReturn);
			return funcResultReturn;
		}

		/// <summary>
		/// Returns the number of audio output devices available in the system.
		/// 
		/// The returned value should not be cached and should instead be used immediately with the <see cref="GetAudioOutputDeviceByIndex" />
		/// function.
		/// <seealso cref="GetAudioOutputDeviceByIndex" />
		/// <seealso cref="AddNotifyAudioDevicesChanged" />
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation</param>
		/// <returns>
		/// The number of audio output devices
		/// </returns>
		public uint GetAudioOutputDevicesCount(ref GetAudioOutputDevicesCountOptions options)
		{
			GetAudioOutputDevicesCountOptionsInternal optionsInternal = new GetAudioOutputDevicesCountOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_GetAudioOutputDevicesCount(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			return funcResult;
		}

		/// <summary>
		/// Use this function to inform the audio system of a user.
		/// 
		/// This function is only necessary for some platforms.
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the user was successfully registered, <see cref="Result.UnexpectedError" /> otherwise.
		/// </returns>
		public Result RegisterPlatformAudioUser(ref RegisterPlatformAudioUserOptions options)
		{
			RegisterPlatformAudioUserOptionsInternal optionsInternal = new RegisterPlatformAudioUserOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_RegisterPlatformAudioUser(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			return funcResult;
		}

		/// <summary>
		/// Unregister a previously bound notification handler from receiving remote audio buffers before they are rendered.
		/// </summary>
		/// <param name="notificationId">The Notification ID representing the registered callback</param>
		public void RemoveNotifyAudioBeforeRender(ulong notificationId)
		{
			Bindings.EOS_RTCAudio_RemoveNotifyAudioBeforeRender(InnerHandle, notificationId);

			Helper.RemoveCallbackByNotificationId(notificationId);
		}

		/// <summary>
		/// Unregister a previously bound notification handler from receiving local audio buffers before they are encoded and sent.
		/// </summary>
		/// <param name="notificationId">The Notification ID representing the registered callback</param>
		public void RemoveNotifyAudioBeforeSend(ulong notificationId)
		{
			Bindings.EOS_RTCAudio_RemoveNotifyAudioBeforeSend(InnerHandle, notificationId);

			Helper.RemoveCallbackByNotificationId(notificationId);
		}

		/// <summary>
		/// Unregister a previously bound notification handler from receiving audio devices notifications
		/// </summary>
		/// <param name="notificationId">The Notification ID representing the registered callback</param>
		public void RemoveNotifyAudioDevicesChanged(ulong notificationId)
		{
			Bindings.EOS_RTCAudio_RemoveNotifyAudioDevicesChanged(InnerHandle, notificationId);

			Helper.RemoveCallbackByNotificationId(notificationId);
		}

		/// <summary>
		/// Unregister a previously bound notification handler from receiving notifications on audio input state changed.
		/// </summary>
		/// <param name="notificationId">The Notification ID representing the registered callback</param>
		public void RemoveNotifyAudioInputState(ulong notificationId)
		{
			Bindings.EOS_RTCAudio_RemoveNotifyAudioInputState(InnerHandle, notificationId);

			Helper.RemoveCallbackByNotificationId(notificationId);
		}

		/// <summary>
		/// Unregister a previously bound notification handler from receiving notifications on audio output state changed.
		/// </summary>
		/// <param name="notificationId">The Notification ID representing the registered callback</param>
		public void RemoveNotifyAudioOutputState(ulong notificationId)
		{
			Bindings.EOS_RTCAudio_RemoveNotifyAudioOutputState(InnerHandle, notificationId);

			Helper.RemoveCallbackByNotificationId(notificationId);
		}

		/// <summary>
		/// Unregister a previously bound notification handler from receiving participant updated notifications
		/// </summary>
		/// <param name="notificationId">The Notification ID representing the registered callback</param>
		public void RemoveNotifyParticipantUpdated(ulong notificationId)
		{
			Bindings.EOS_RTCAudio_RemoveNotifyParticipantUpdated(InnerHandle, notificationId);

			Helper.RemoveCallbackByNotificationId(notificationId);
		}

		/// <summary>
		/// Use this function to push a new audio buffer to be sent to the participants of a room.
		/// 
		/// This should only be used if Manual Audio Input was enabled locally for the specified room.
		/// <seealso cref="RTC.JoinRoomOptions" />
		/// <seealso cref="Lobby.LocalRTCOptions" />
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the buffer was successfully queued for sending
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// <see cref="Result.NotFound" /> if the specified room was not found
		/// <see cref="Result.InvalidState" /> if manual recording was not enabled when joining the room.
		/// </returns>
		public Result SendAudio(ref SendAudioOptions options)
		{
			SendAudioOptionsInternal optionsInternal = new SendAudioOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_SendAudio(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			return funcResult;
		}

		/// <summary>
		/// Use this function to set audio input settings, such as the active input device, volume, or platform AEC.
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the setting was successful
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// </returns>
		public Result SetAudioInputSettings(ref SetAudioInputSettingsOptions options)
		{
			SetAudioInputSettingsOptionsInternal optionsInternal = new SetAudioInputSettingsOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_SetAudioInputSettings(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			return funcResult;
		}

		/// <summary>
		/// Use this function to set audio output settings, such as the active output device or volume.
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the setting was successful
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// </returns>
		public Result SetAudioOutputSettings(ref SetAudioOutputSettingsOptions options)
		{
			SetAudioOutputSettingsOptionsInternal optionsInternal = new SetAudioOutputSettingsOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_SetAudioOutputSettings(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			return funcResult;
		}

		/// <summary>
		/// Use this function to remove a user that was added with <see cref="RegisterPlatformAudioUser" />.
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the user was successfully unregistered, <see cref="Result.UnexpectedError" /> otherwise.
		/// </returns>
		public Result UnregisterPlatformAudioUser(ref UnregisterPlatformAudioUserOptions options)
		{
			UnregisterPlatformAudioUserOptionsInternal optionsInternal = new UnregisterPlatformAudioUserOptionsInternal();
			optionsInternal.Set(ref options);

			var funcResult = Bindings.EOS_RTCAudio_UnregisterPlatformAudioUser(InnerHandle, ref optionsInternal);

			Helper.Dispose(ref optionsInternal);

			return funcResult;
		}

		/// <summary>
		/// Use this function to change participant audio volume for a room.
		/// Due to internal implementation details, this function requires that you first register to any notification for room
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when the operation completes, either successfully or in error</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the operation succeeded
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// <see cref="Result.NotFound" /> if either the local user or specified participant are not in the room
		/// </returns>
		public void UpdateParticipantVolume(ref UpdateParticipantVolumeOptions options, object clientData, OnUpdateParticipantVolumeCallback completionDelegate)
		{
			UpdateParticipantVolumeOptionsInternal optionsInternal = new UpdateParticipantVolumeOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnUpdateParticipantVolumeCallbackInternal(OnUpdateParticipantVolumeCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			Bindings.EOS_RTCAudio_UpdateParticipantVolume(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);
		}

		/// <summary>
		/// Use this function to tweak incoming audio options for a room.
		/// Due to internal implementation details, this function requires that you first register to any notification for room
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when the operation completes, either successfully or in error</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the operation succeeded
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// <see cref="Result.NotFound" /> if either the local user or specified participant are not in the room
		/// </returns>
		public void UpdateReceiving(ref UpdateReceivingOptions options, object clientData, OnUpdateReceivingCallback completionDelegate)
		{
			UpdateReceivingOptionsInternal optionsInternal = new UpdateReceivingOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnUpdateReceivingCallbackInternal(OnUpdateReceivingCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			Bindings.EOS_RTCAudio_UpdateReceiving(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);
		}

		/// <summary>
		/// Use this function to change incoming audio volume for a room.
		/// Due to internal implementation details, this function requires that you first register to any notification for room
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when the operation completes, either successfully or on error</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the operation succeeded
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// <see cref="Result.NotFound" /> if the local user is not in the room
		/// </returns>
		public void UpdateReceivingVolume(ref UpdateReceivingVolumeOptions options, object clientData, OnUpdateReceivingVolumeCallback completionDelegate)
		{
			UpdateReceivingVolumeOptionsInternal optionsInternal = new UpdateReceivingVolumeOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnUpdateReceivingVolumeCallbackInternal(OnUpdateReceivingVolumeCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			Bindings.EOS_RTCAudio_UpdateReceivingVolume(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);
		}

		/// <summary>
		/// Use this function to tweak outgoing audio options for a room.
		/// Due to internal implementation details, this function requires that you first register to any notification for room
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when the operation completes, either successfully or in error</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the operation succeeded
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// <see cref="Result.NotFound" /> if the local user is not in the room
		/// </returns>
		public void UpdateSending(ref UpdateSendingOptions options, object clientData, OnUpdateSendingCallback completionDelegate)
		{
			UpdateSendingOptionsInternal optionsInternal = new UpdateSendingOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnUpdateSendingCallbackInternal(OnUpdateSendingCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			Bindings.EOS_RTCAudio_UpdateSending(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);
		}

		/// <summary>
		/// Use this function to change outgoing audio volume for a room.
		/// Due to internal implementation details, this function requires that you first register to any notification for room
		/// </summary>
		/// <param name="options">structure containing the parameters for the operation.</param>
		/// <param name="clientData">Arbitrary data that is passed back in the CompletionDelegate</param>
		/// <param name="completionDelegate">The callback to be fired when the operation completes, either successfully or in error</param>
		/// <returns>
		/// <see cref="Result.Success" /> if the operation succeeded
		/// <see cref="Result.InvalidParameters" /> if any of the parameters are incorrect
		/// <see cref="Result.NotFound" /> if the local user is not in the room
		/// </returns>
		public void UpdateSendingVolume(ref UpdateSendingVolumeOptions options, object clientData, OnUpdateSendingVolumeCallback completionDelegate)
		{
			UpdateSendingVolumeOptionsInternal optionsInternal = new UpdateSendingVolumeOptionsInternal();
			optionsInternal.Set(ref options);

			var clientDataAddress = System.IntPtr.Zero;

			var completionDelegateInternal = new OnUpdateSendingVolumeCallbackInternal(OnUpdateSendingVolumeCallbackInternalImplementation);
			Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

			Bindings.EOS_RTCAudio_UpdateSendingVolume(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

			Helper.Dispose(ref optionsInternal);
		}

		[MonoPInvokeCallback(typeof(OnAudioBeforeRenderCallbackInternal))]
		internal static void OnAudioBeforeRenderCallbackInternalImplementation(ref AudioBeforeRenderCallbackInfoInternal data)
		{
			OnAudioBeforeRenderCallback callback;
			AudioBeforeRenderCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnAudioBeforeSendCallbackInternal))]
		internal static void OnAudioBeforeSendCallbackInternalImplementation(ref AudioBeforeSendCallbackInfoInternal data)
		{
			OnAudioBeforeSendCallback callback;
			AudioBeforeSendCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnAudioDevicesChangedCallbackInternal))]
		internal static void OnAudioDevicesChangedCallbackInternalImplementation(ref AudioDevicesChangedCallbackInfoInternal data)
		{
			OnAudioDevicesChangedCallback callback;
			AudioDevicesChangedCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnAudioInputStateCallbackInternal))]
		internal static void OnAudioInputStateCallbackInternalImplementation(ref AudioInputStateCallbackInfoInternal data)
		{
			OnAudioInputStateCallback callback;
			AudioInputStateCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnAudioOutputStateCallbackInternal))]
		internal static void OnAudioOutputStateCallbackInternalImplementation(ref AudioOutputStateCallbackInfoInternal data)
		{
			OnAudioOutputStateCallback callback;
			AudioOutputStateCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnParticipantUpdatedCallbackInternal))]
		internal static void OnParticipantUpdatedCallbackInternalImplementation(ref ParticipantUpdatedCallbackInfoInternal data)
		{
			OnParticipantUpdatedCallback callback;
			ParticipantUpdatedCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnUpdateParticipantVolumeCallbackInternal))]
		internal static void OnUpdateParticipantVolumeCallbackInternalImplementation(ref UpdateParticipantVolumeCallbackInfoInternal data)
		{
			OnUpdateParticipantVolumeCallback callback;
			UpdateParticipantVolumeCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnUpdateReceivingCallbackInternal))]
		internal static void OnUpdateReceivingCallbackInternalImplementation(ref UpdateReceivingCallbackInfoInternal data)
		{
			OnUpdateReceivingCallback callback;
			UpdateReceivingCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnUpdateReceivingVolumeCallbackInternal))]
		internal static void OnUpdateReceivingVolumeCallbackInternalImplementation(ref UpdateReceivingVolumeCallbackInfoInternal data)
		{
			OnUpdateReceivingVolumeCallback callback;
			UpdateReceivingVolumeCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnUpdateSendingCallbackInternal))]
		internal static void OnUpdateSendingCallbackInternalImplementation(ref UpdateSendingCallbackInfoInternal data)
		{
			OnUpdateSendingCallback callback;
			UpdateSendingCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}

		[MonoPInvokeCallback(typeof(OnUpdateSendingVolumeCallbackInternal))]
		internal static void OnUpdateSendingVolumeCallbackInternalImplementation(ref UpdateSendingVolumeCallbackInfoInternal data)
		{
			OnUpdateSendingVolumeCallback callback;
			UpdateSendingVolumeCallbackInfo callbackInfo;
			if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
			{
				callback(ref callbackInfo);
			}
		}
	}
}