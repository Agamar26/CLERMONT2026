using System.Globalization;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class networkManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public GameObject playerprefab;
    public GameObject eyeprefab;
    public float speed;
    public Vector2 move;
    public enum stateGame { attentenewjoueur,compteuravantlancement, launchball,gamerunning , respawnball }
    public stateGame state;
    public GameObject ball;
    public float launchTime, launchTimer;
    public static networkManager instance;
    public string currentstateName;
    public TextMeshProUGUI textTimer;
    void Start()
    {
        // Fait spawner le joueur local dès qu'il arrive dans la scène (si déjà dans une salle)
        if (PhotonNetwork.InRoom)
        {
            AssignPlayerSlot();
        }
    }
    private void Update()
    {
        if(instance == null)
        {
            instance = this;
        }
        switch (state)
        {
            case stateGame.attentenewjoueur:
                textTimer.text = "En attente d'un autre joueur...";
                var dsds = GameObject.FindGameObjectsWithTag("Player");
                if (dsds.Length == 2)
                {
                    if(PhotonNetwork.IsMasterClient)
                    {
                        launchTimer = 0;
                        ball = PhotonNetwork.Instantiate(eyeprefab.name, Vector2.zero, Quaternion.identity); 
                        networkManager.instance.currentstateName = "compteuravantlancement";
                        state = stateGame.compteuravantlancement;
                    }
                    
                    
                }
                break;


            case stateGame.compteuravantlancement:
                textTimer.text = launchTimer.ToString("0");
                if (PhotonNetwork.IsMasterClient)
                {
                    if (launchTimer != launchTime)
                    {
                        launchTimer = Mathf.MoveTowards(launchTimer, launchTime, Time.deltaTime);
                    }
                    if (launchTimer == launchTime)
                    {
                        networkManager.instance.currentstateName = "launchball";
                        state = stateGame.launchball;
                    }
                }
                
                break;

            case stateGame.launchball:
                textTimer.text = "";
                if (PhotonNetwork.IsMasterClient)
                {
                    ball.GetComponent<eyescriptNet>().launchBall();
                }
                networkManager.instance.currentstateName = "gamerunning";
                state = stateGame.gamerunning;
                break;

            case stateGame.gamerunning:
                textTimer.text = "";
                break;
            case stateGame.respawnball:
                textTimer.text = "";
                if(PhotonNetwork.IsMasterClient)
                {
                    launchTimer = 0;
                    ball = PhotonNetwork.Instantiate(eyeprefab.name, Vector2.zero, Quaternion.identity);
                    networkManager.instance.currentstateName = "compteuravantlancement";
                    state = stateGame.compteuravantlancement;
                }
              
                break;
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            stateGame targetState = (stateGame)System.Enum.Parse(typeof(stateGame), currentstateName);

            // 2. Ensuite, comparez directement les enums sans refaire de Parse :
            if (state != targetState)
            {
                state = targetState;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            var dsds = GameObject.FindGameObjectsWithTag("Player");
            if (dsds.Length != 2)
            {
               
            }
        }

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (state != stateGame.attentenewjoueur)
        {
            var ez = GameObject.FindGameObjectWithTag("Oeil");
            PhotonNetwork.Destroy(ez);

            networkManager.instance.currentstateName = "attentenewjoueur";
            state = stateGame.attentenewjoueur;
        }

    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
    }

    void AssignPlayerSlot()
    {
        int assignedSlot = -1;

        // On regarde quel slot est libre (0 ou 1)
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.ContainsKey("PlayerSlot"))
            {
                int slot = (int)p.CustomProperties["PlayerSlot"];
                if (slot == 0) assignedSlot = 1; // Si 0 est pris, on prend 1
            }
        }

        if (assignedSlot == -1) assignedSlot = 0; // Par défaut le premier prend 0

        // On sauvegarde cette info dans les propriétés du joueur sur le réseau
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { "PlayerSlot", assignedSlot } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Une fois assigné, on fait spawner le joueur
        SpawnPlayer(assignedSlot);
    }
    void SpawnPlayer(int slot)
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (slot == 0)
        {
            spawnPosition = new Vector3(-8.2f, 0, 0);
            spawnRotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            spawnPosition = new Vector3(8.2f, 0, 0);
            spawnRotation = Quaternion.Euler(0, 0, 90);
        }

        PhotonNetwork.Instantiate(playerprefab.name, spawnPosition, spawnRotation);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // C'est nous le propriétaire : on envoie nos données aux autres
            stream.SendNext(launchTimer);
            stream.SendNext(currentstateName);
        }
        else
        {
            // C'est un autre joueur : on reçoit ses données
            launchTimer = (float)stream.ReceiveNext();
            currentstateName = (string)stream.ReceiveNext();

        }
    }
}
