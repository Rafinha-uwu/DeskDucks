using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.UTP;

public class MultiplayerMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField joinInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text roomCodeText;

    private readonly StringBuilder statusBuilder = new();
    private bool servicesReady;

    private async void Start()
    {
        RegisterFishNetCallbacks();
        await InitUnityServices();
    }

    private void OnDestroy()
    {
        UnregisterFishNetCallbacks();
    }

    private void RegisterFishNetCallbacks()
    {
        if (networkManager == null)
            return;

        networkManager.ServerManager.OnServerConnectionState += HandleServerState;
        networkManager.ClientManager.OnClientConnectionState += HandleClientState;
    }

    private void UnregisterFishNetCallbacks()
    {
        if (networkManager == null)
            return;

        networkManager.ServerManager.OnServerConnectionState -= HandleServerState;
        networkManager.ClientManager.OnClientConnectionState -= HandleClientState;
    }

    private void HandleServerState(ServerConnectionStateArgs args)
    {
        AppendStatus("SERVER STATE: " + args.ConnectionState);
    }

    private void HandleClientState(ClientConnectionStateArgs args)
    {
        AppendStatus("CLIENT STATE: " + args.ConnectionState);
    }

    private async Task InitUnityServices()
    {
        try
        {
            servicesReady = false;

            AppendStatus("Initializing Unity Services...");
            await UnityServices.InitializeAsync();
            AppendStatus("Unity Services initialized.");

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                AppendStatus("Signing in anonymously...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            AppendStatus("Signed in. PlayerID: " + AuthenticationService.Instance.PlayerId);
            servicesReady = true;
            AppendStatus("Ready.");
        }
        catch (System.Exception e)
        {
            servicesReady = false;
            AppendStatus("Init failed: " + e.Message);
            Debug.LogException(e);
        }
    }

    public async void CreateRoom()
    {
        if (!servicesReady)
        {
            AppendStatus("CreateRoom blocked: services are not ready yet.");
            return;
        }

        try
        {
            AppendStatus("Creating Relay allocation...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(8);
            AppendStatus("Allocation created.");

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            AppendStatus("Join code: " + joinCode);

            if (roomCodeText != null)
                roomCodeText.text = joinCode;

            UnityTransport transport = GetUnityTransport();
            if (transport == null)
                return;

            AppendStatus("Configuring Relay host data...");
            transport.UseWebSockets = false;
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            AppendStatus("Starting server...");
            bool serverStarted = networkManager.ServerManager.StartConnection();
            AppendStatus("Server start returned: " + serverStarted);

            AppendStatus("Starting host client...");
            bool clientStarted = networkManager.ClientManager.StartConnection();
            AppendStatus("Host client start returned: " + clientStarted);
        }
        catch (System.Exception e)
        {
            AppendStatus("CreateRoom failed: " + e.Message);
            Debug.LogException(e);
        }
    }

    public async void JoinRoom()
    {
        if (!servicesReady)
        {
            AppendStatus("JoinRoom blocked: services are not ready yet.");
            return;
        }

        try
        {
            string code = joinInput != null ? joinInput.text.Trim().ToUpper() : string.Empty;

            if (string.IsNullOrWhiteSpace(code))
            {
                AppendStatus("JoinRoom failed: code is empty.");
                return;
            }

            AppendStatus("Joining with code: " + code);

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);
            AppendStatus("Join allocation received.");

            UnityTransport transport = GetUnityTransport();
            if (transport == null)
                return;

            AppendStatus("Configuring Relay client data...");
            transport.UseWebSockets = false;
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            AppendStatus("Starting client...");
            bool clientStarted = networkManager.ClientManager.StartConnection();
            AppendStatus("Client start returned: " + clientStarted);
        }
        catch (System.Exception e)
        {
            AppendStatus("JoinRoom failed: " + e.Message);
            Debug.LogException(e);
        }
    }

    public void StopAllConnections()
    {
        if (networkManager == null)
            return;

        AppendStatus("Stopping all connections...");

        if (networkManager.ClientManager.Started)
            networkManager.ClientManager.StopConnection();

        if (networkManager.ServerManager.Started)
            networkManager.ServerManager.StopConnection(true);
    }

    private UnityTransport GetUnityTransport()
    {
        if (networkManager == null)
        {
            AppendStatus("ERROR: NetworkManager reference is missing.");
            return null;
        }

        UnityTransport transport = networkManager.TransportManager.Transport as UnityTransport;

        if (transport == null)
        {
            AppendStatus("ERROR: Active transport is not UnityTransport.");
            return null;
        }

        return transport;
    }

    private void AppendStatus(string message)
    {
        Debug.Log(message);

        if (statusBuilder.Length > 0)
            statusBuilder.AppendLine();

        statusBuilder.Append(message);

        if (statusText != null)
            statusText.text = statusBuilder.ToString();
    }
}