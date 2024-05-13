using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float damages = 10;
    public Photon.Realtime.Player shooter;

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
    }

    private void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 50f);
    }

    [PunRPC]
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    [PunRPC]
    public void SetShooter(int shooterID)
    {
        shooter = PhotonNetwork.CurrentRoom.GetPlayer(shooterID);
    }
}