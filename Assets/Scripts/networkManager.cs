using System.Collections;
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
           StartCoroutine(SpawnPlayer());
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
           // var dsds = GameObject.FindGameObjectsWithTag("Player");
            //if (dsds.Length != 2)
           // {
               
          //  }
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
        // Sécurité si on rejoint la salle après le chargement de la scène
        StartCoroutine(SpawnPlayer());
    }

    public IEnumerator SpawnPlayer()
    {
        // Récupère le nombre total de joueurs pour déterminer la position
        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount;
        Vector3 spawnPosition = Vector2.zero;
        Quaternion spawnRotation = Quaternion.identity;
        if (playerIndex == 1)
        {
            spawnPosition = new Vector3(-8.2f, 0, 0);
            spawnRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (playerIndex == 2)
        {
            print("loloololo");
            var ez = GameObject.FindGameObjectWithTag("Player");
            while (ez == null)
            {
                
                ez = GameObject.FindGameObjectWithTag("Player");
                yield return null;
            }
            // Alternance gauche / droite selon l'ordre d'arrivée
            if (ez.transform.position.x > 0)
            {
                spawnPosition = new Vector3(-8.2f, 0, 0);
                spawnRotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                spawnPosition = new Vector3(8.2f, 0, 0);
                spawnRotation = Quaternion.Euler(0, 0, 90);
            }
        }
        // Chaque client instancie son propre personnage sur le réseau
        var ese = PhotonNetwork.Instantiate(playerprefab.name, spawnPosition, spawnRotation);
        



        yield break;
       
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
