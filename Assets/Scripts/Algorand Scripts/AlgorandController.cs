using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorand.Unity;
using Algorand.Unity.WalletConnect;

public class AlgorandController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    /*
    public void OnClickConnectWallet()
    {
        // 1. Create the session and show the user the QR Code
        var dappMeta = new ClientMeta
        {
            Name = "<name of your dapp>",
            Description = "<description of your dapp>",
            Url = "<url of your dapp>",
            IconUrls = new[]
            {
                "<icon1 of your dapp>", "<icon2 of your dapp>"
            }
        };
        var session = new AlgorandWalletConnectSession(dappMeta);
        await session.Connect();
        // session is in no connection status
        Debug.Assert(session.ConnectionStatus == SessionStatus.NoWalletConnected);

        var handshake = session.RequestWalletConnection();
        // session should now be in connecting status
        Debug.Assert(session.ConnectionStatus == SessionStatus.RequestingWalletConnection);

        // show the user a QR Code
        Texture2D qrCode = handshake.ToQrCodeTexture();
        // OR launch a supported wallet app
        WalletRegistry.PeraWallet.LaunchForConnect(handshake);

        // 2. Wait for user to approve the connection
        await session.WaitForWalletApproval();
        // session is now connected
        Debug.Assert(session.ConnectionStatus == SessionStatus.WalletConnected);
    }

    */
}
