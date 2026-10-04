using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.U2D.Physics.PhysicsLayers;

public class networkManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public GameObject playerprefab;
    public GameObject eyeprefab;
    public float speed;
    public Vector2 move;
    public enum stateGame { attentenewjoueur,compteuravantlancement, launchball,gamerunning }
    public stateGame state;
    public GameObject ball;
    public float launchTime, launchTimer;
    
    void Start()
    {
        // Fait spawner le joueur local dès qu'il arrive dans la scène (si déjà dans une salle)
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }
    private void Update()
    {
        
        switch (state)
        {
            case stateGame.attentenewjoueur:
                var dsds = GameObject.FindGameObjectsWithTag("Player");
                if (dsds.Length == 2)
                {
                    if(PhotonNetwork.IsMasterClient)
                    {
                        ball = PhotonNetwork.Instantiate(eyeprefab.name, Vector2.zero, Quaternion.identity);
                    }

                    state = stateGame.compteuravantlancement;
                    
                }
                break;


            case stateGame.compteuravantlancement:
                if(PhotonNetwork.IsMasterClient)
                {
                    if (launchTimer != launchTime)
                    {
                        launchTimer = Mathf.MoveTowards(launchTimer, launchTime, Time.deltaTime);
                    }   
                }
                if(launchTimer == launchTime)
                {
                    state = stateGame.launchball;
                }
                break;

            case stateGame.launchball:
                if (PhotonNetwork.IsMasterClient)
                {
                    ball.GetComponent<eyescriptNet>().launchBall();
                }
                state = stateGame.gamerunning;
                break;

            case stateGame.gamerunning:

                break;


        }
       
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        // Sécurité si on rejoint la salle après le chargement de la scène
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // Récupère le nombre total de joueurs pour déterminer la position
        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount;

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        // Alternance gauche / droite selon l'ordre d'arrivée
        if (playerIndex % 2 != 0)
        {
            spawnPosition = new Vector3(-8.2f, 0, 0);
            spawnRotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            spawnPosition = new Vector3(8.2f, 0, 0);
            spawnRotation = Quaternion.Euler(0, 0, 90);
        }

        // Chaque client instancie son propre personnage sur le réseau
        PhotonNetwork.Instantiate(playerprefab.name, spawnPosition, spawnRotation);

       
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // C'est nous le propriétaire : on envoie nos données aux autres
            stream.SendNext(launchTimer);
        }
        else
        {
            // C'est un autre joueur : on reçoit ses données
            launchTimer = (float)stream.ReceiveNext();

        }
    }
}
