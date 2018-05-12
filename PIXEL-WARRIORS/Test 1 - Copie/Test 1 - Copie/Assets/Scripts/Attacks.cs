﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attacks : MonoBehaviour
{
    public GameObject basic1;
    public GameObject basic2;
    public GameObject basic3;
    public GameObject special1;

    public Image cooldown1;
    public Image cooldown2;
    public Image cooldown3;

    public bool muteSound;

    private float coolDownTime;

    public Vector2 velocity_special1;

    public Vector2 offset_basic1;
    public Vector2 offset_basic2;
    public Vector2 offset_basic3;
    public Vector2 offset_special1;

    public float cooldown_special1;
    public float cooldown_special2;
    public float cooldown_special3;

    private float direction = 0f;
    private float rotation = 0;

    private float lifeTime_special1 = 2f;
    private float lifeTime_special2 = 2f;
    private float lifeTime_special3 = 3f;

    bool canShoot_basic = true;
    bool canShootSp1 = true;
    bool canShootSp2 = true;
    bool canShootSp3 = true;
    private bool attacking;

    private float cooldown = 0.3f;
    private float lifeTime = 0.3f;
    private float damage;

    private Player player;
    private Rigidbody2D rb2d;
    private Animator anim;
    private Manager manager;
    private new AudioManager audio;

    private bool earlyShoot = false;

    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        player = gameObject.GetComponentInParent<Player>();
        anim = gameObject.GetComponent<Animator>();
        manager = gameObject.GetComponent<Manager>();
        audio = FindObjectOfType<AudioManager>();

        //Debug.Log(player.muteSound);
        //muteSound = player.muteSound;
        muteSound = audio.soundOn;
    }

    #region Coroutines
    IEnumerator CanShootBasic()
    {
        canShoot_basic = false;
        yield return new WaitForSeconds(cooldown);
        canShoot_basic = true;
    }
    IEnumerator CanShootSpecial1() //Y
    {
        canShootSp1 = false;
        yield return new WaitForSeconds(cooldown_special1);
        player.manager.GetComponent<Manager>().coolingDown1 = false;
        canShootSp1 = true;
    }
    IEnumerator CanShootSpecial2() //W-Y
    {
        canShootSp2 = false;
        yield return new WaitForSeconds(cooldown_special2);
        player.manager.GetComponent<Manager>().coolingDown2 = false;
        canShootSp2 = true;
    }
    IEnumerator CanShootSpecial3() //s-y
    {
        canShootSp3 = false;
        yield return new WaitForSeconds(cooldown_special3);
        player.manager.GetComponent<Manager>().coolingDown3 = false;
        canShootSp3 = true;
    }
    IEnumerator DelayAnim()
    {
        yield return new WaitForSeconds(0.25f);
        player.special_1 = false;
        player.special_2 = false;
        player.special_3 = false;
    }
    #endregion

    private void TaggingBasic(GameObject go, int playerNumber)
    {
        if (playerNumber == 1)
        {
            go.tag = "AttPlayer1";
            go.layer = 11;
        }
        else if (playerNumber == 2)
        {
            go.tag = "AttPlayer2";
            go.layer = 12;
        }

        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), go.GetComponent<Collider2D>());
        Destroy(go, lifeTime);
        rotation = 0;
        StartCoroutine("CanShootBasic");
    }

    public void LaunchBasic1(int playerNum) //A
    {
        //Basic1
        if (canShoot_basic && !player.dead)
        {
            damage = 0.25f;
            GameObject go;

            audio.Play("Whoosh", 0, muteSound);

            if (player.name == "Scientist(Clone)" || player.name == "Demon(Clone)")
            {
                go = (GameObject)Instantiate(basic1, (Vector2)transform.position + (new Vector2(offset_basic1.x * transform.localScale.x, offset_basic1.y)), Quaternion.Euler(0, 0, 0));
                TaggingBasic(go, playerNum);
            }
            if (player.name == "Alien(Clone)" || player.name == "Ninja(Clone)")
            {
                if (!player.isRight)
                {
                    rotation = 180;
                }
                go = (GameObject)Instantiate(basic1, (Vector2)transform.position + (new Vector2(offset_basic1.x * transform.localScale.x, offset_basic1.y)), Quaternion.Euler(0, rotation, 0));
                TaggingBasic(go, playerNum);
            }
        }
    }

    public void LaunchBasic2(int playerNum) //A + ↑
    {
        //Basic2
        if (canShoot_basic && !player.dead)
        {
            damage = 0.5f;


            if (!player.isRight)
            {
                rotation = 180;
            }
            GameObject go = (GameObject)Instantiate(basic2, (Vector2)transform.position + (new Vector2(offset_basic2.x * transform.localScale.x, offset_basic2.y)), Quaternion.Euler(0, rotation, 0));
            TaggingBasic(go, playerNum);

            audio.Play("Whoosh", 0, muteSound);

        }
    }

    public void LaunchBasic3(int playerNum) //A + ↓
    {
        //Basic3
        if (canShoot_basic && !player.dead)
        {
            damage = 0.5f;


            if (!player.isRight)
            {
                rotation = 180;
            }
            GameObject go = (GameObject)Instantiate(basic3, (Vector2)transform.position + (new Vector2(offset_basic3.x * transform.localScale.x, offset_basic3.y)), Quaternion.Euler(0, rotation, 0));
            TaggingBasic(go, playerNum);

            audio.Play("Whoosh", 0, muteSound);

        }
    }

    public void LaunchSpecial1(int playerNum, bool state, float chargePercentage) //B
    {
        if (canShootSp1 && !player.dead)
        {
            if (state)
            {
                audio.Play("Charge", 0, muteSound);

                player.charge = true;
                anim.SetBool("Charge", true);
            }
            else if (player.charge)
            {
                //Debug.Log("Charge = " + chargePercentage);

                audio.Stop("Charge", 0, muteSound);

                player.charge = false;
                anim.SetBool("Charge", false);
                if (player.isRight)
                {
                    direction = 0f;
                }
                else { direction = 180f; }
                damage = 0.75f;
                if (player.name == "Scientist(Clone)") { rotation = 90; }
                else { rotation = 0; }

                player.manager.GetComponent<Manager>().coolingDown1 = true;
                GameObject go = (GameObject)Instantiate(special1, (Vector2)transform.position + offset_special1 * transform.localScale.x, Quaternion.Euler(0, direction, rotation));

                if (chargePercentage < 1) { damage = 0.1f; }
                if (chargePercentage > 1 && chargePercentage < 2) { damage = 0.2f; }
                if (chargePercentage > 2) { damage = 0.3f; }
                go.GetComponent<Damage>().damage = damage;

                if (playerNum == 1)
                {
                    go.tag = "AttPlayer1";
                    go.layer = 11;
                }
                else if (playerNum == 2)
                {
                    go.tag = "AttPlayer2";
                    go.layer = 12;
                }
                go.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity_special1.x * transform.localScale.x, velocity_special1.y);
                GetComponent<Animator>().SetTrigger("Special1");
                Destroy(go, lifeTime_special1);
                StartCoroutine("CanShootSpecial1");

                audio.Play("Special1", 0, muteSound);

            }
        }

    }

    public void LaunchSpecial2(int playerNum) //B + ↑
    {
        StartCoroutine("DelayAnim");
        if (player.name == "Demon(Clone)" && canShootSp2 && !player.dead && player.grounded)
        {
            player.manager.GetComponent<Manager>().coolingDown2 = true;
            damage = 1.25f;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            float x = player.GetComponent<Rigidbody2D>().position.x;
            float y = player.GetComponent<Rigidbody2D>().position.y;

            GameObject smallBoneLeft = Instantiate(Resources.Load("Demon_Small_Bone"), new Vector2(x - 0.58499652f, y - 0.07370224f), Quaternion.identity) as GameObject;
            GameObject smallBoneRight = Instantiate(Resources.Load("Demon_Small_Bone"), new Vector2(x + 0.58499652f, y - 0.07370224f), Quaternion.identity) as GameObject;
            smallBoneRight.transform.localScale = new Vector2(-1.187455f, 1.187455f);

            GameObject smallColLeft = Instantiate(Resources.Load("Demon_Small_Col"), new Vector2(x - 1.1105336f, y - 0.34870224f), Quaternion.identity) as GameObject;
            GameObject smallColRight = Instantiate(Resources.Load("Demon_Small_Col"), new Vector2(x + 1.1105336f, y - 0.34870224f), Quaternion.identity) as GameObject;
            smallColRight.transform.localScale = new Vector2(-1, 1);

            Vector3 lFrom = smallColLeft.transform.position;
            Vector3 lTo = smallColLeft.transform.position + new Vector3(0, 0.375f, 0);
            Vector3 rFrom = smallColRight.transform.position;
            Vector3 rTo = smallColRight.transform.position + new Vector3(0, 0.375f, 0);

            StartCoroutine(Move(smallColLeft, smallColRight, lFrom, lTo, rFrom, rTo));


            if (playerNum == 1)
            {
                smallColLeft.tag = "AttPlayer1";
                smallColLeft.layer = 11;
                smallColRight.tag = "AttPlayer1";
                smallColRight.layer = 11;
            }
            else if (playerNum == 2)
            {
                smallColLeft.tag = "AttPlayer2";
                smallColLeft.layer = 12;
                smallColRight.tag = "AttPlayer2";
                smallColRight.layer = 12;
            }

            StartCoroutine(DemonBones(playerNum, x, y));

            Destroy(smallBoneLeft, 0.5f);
            Destroy(smallBoneRight, 0.5f);
            Destroy(smallColLeft, 0.5f);
            Destroy(smallColRight, 0.5f);

            StartCoroutine("CanShootSpecial2");

            audio.Play("SpikeBones", 0, muteSound);

        }

        if (player.name == "Alien(Clone)" && canShootSp2 && !player.dead)
        {
            player.manager.GetComponent<Manager>().coolingDown2 = true;
            float posBallUpBottom;
            float posBallMiddle;
            Vector2 ballSpeedUp;
            Vector2 ballSpeedMiddle;
            Vector2 ballSpeedBottom;

            if (player.isRight)
            {
                direction = 0f;
                posBallUpBottom = 0.24100546f;
                posBallMiddle = 0.33900546f;
                ballSpeedUp = new Vector2(3.5f, 0.3f);
                ballSpeedMiddle = new Vector2(3.5f, 0);
                ballSpeedBottom = new Vector2(3.5f, -0.3f);
            }
            else
            {
                direction = 180f;
                posBallUpBottom = -0.24100546f;
                posBallMiddle = -0.33900546f;
                ballSpeedUp = new Vector2(-3.5f, 0.3f);
                ballSpeedMiddle = new Vector2(-3.5f, 0);
                ballSpeedBottom = new Vector2(-3.5f, -0.3f);
            }
            damage = 1.25f;



            GameObject ballUp = Instantiate(Resources.Load("Alien_TriBall"), new Vector2(player.GetComponent<Rigidbody2D>().position.x + posBallUpBottom, player.GetComponent<Rigidbody2D>().position.y + 0.15663729f), Quaternion.Euler(0, direction, 0)) as GameObject;
            GameObject ballMiddle = Instantiate(Resources.Load("Alien_TriBall"), new Vector2(player.GetComponent<Rigidbody2D>().position.x + posBallMiddle, player.GetComponent<Rigidbody2D>().position.y + 0.00763729f), Quaternion.Euler(0, direction, 0)) as GameObject;
            GameObject ballBottom = Instantiate(Resources.Load("Alien_TriBall"), new Vector2(player.GetComponent<Rigidbody2D>().position.x + posBallUpBottom, player.GetComponent<Rigidbody2D>().position.y - 0.12336271f), Quaternion.Euler(0, direction, 0)) as GameObject;
            if (playerNum == 1)
            {
                ballUp.tag = "AttPlayer1";
                ballUp.layer = 11;
                ballMiddle.tag = "AttPlayer1";
                ballMiddle.layer = 11;
                ballBottom.tag = "AttPlayer1";
                ballBottom.layer = 11;
            }
            else if (playerNum == 2)
            {
                ballUp.tag = "AttPlayer2";
                ballUp.layer = 12;
                ballMiddle.tag = "AttPlayer2";
                ballMiddle.layer = 12;
                ballBottom.tag = "AttPlayer2";
                ballBottom.layer = 12;
            }
            ballUp.GetComponent<Rigidbody2D>().velocity = ballSpeedUp;
            ballMiddle.GetComponent<Rigidbody2D>().velocity = ballSpeedMiddle;
            ballBottom.GetComponent<Rigidbody2D>().velocity = ballSpeedBottom;

            Destroy(ballUp, 3);
            Destroy(ballMiddle, 3);
            Destroy(ballBottom, 3);

            StartCoroutine("CanShootSpecial2");


            audio.Play("TriBall", 0, muteSound);
            audio.Play("TriBall", 0.1f, muteSound);
            audio.Play("TriBall", 0.2f, muteSound);
        }

        if (player.name == "Ninja(Clone)" && canShootSp2 && !player.dead)
        {
            player.manager.GetComponent<Manager>().coolingDown2 = true;
            damage = 1.25f;
            Vector2 offset;
            Vector2 velocity = new Vector2(1.8f, 2.3f);
            if (player.isRight)
            {
                direction = 0f;
                offset = new Vector2(0.2f, 0.1f);
            }
            else
            {
                direction = 180f;
                offset = new Vector2(0.2f, -0.1f);
            }


            GameObject bomb = (GameObject)Instantiate(Resources.Load("Ninja_Bomb"), (Vector2)transform.position + offset * transform.localScale.x, Quaternion.Euler(0, direction, 0));
            if (playerNum == 1)
            {
                bomb.tag = "AttPlayer1";
                bomb.layer = 11;
            }
            else if (playerNum == 2)
            {
                bomb.tag = "AttPlayer2";
                bomb.layer = 12;
            }
            bomb.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);
            StartCoroutine("BombBlast", bomb);
            StartCoroutine("CanShootSpecial2");

            audio.Play("Throw", 0, muteSound);

        }

        if (player.name == "Scientist(Clone)" && canShootSp2 && !player.dead)
        {
            player.manager.GetComponent<Manager>().coolingDown2 = true;
            damage = 1.25f;
            Vector2 offset;
            Vector2 velocity = new Vector2(1.8f, 2.3f);
            if (player.isRight)
            {
                direction = 0f;
                offset = new Vector2(0.2f, 0.1f);
            }
            else
            {
                direction = 180f;
                offset = new Vector2(0.2f, -0.1f);
            }

            audio.Play("Throw", 0, muteSound);

            GameObject potionPoison = (GameObject)Instantiate(Resources.Load("Scientist_Potion_Poison"), (Vector2)transform.position + offset * transform.localScale.x, Quaternion.Euler(0, direction, 0));
            if (playerNum == 1)
            {
                potionPoison.GetComponent<PotionPoison>().playerNum = 1;
            }
            else if (playerNum == 2)
            {
                potionPoison.GetComponent<PotionPoison>().playerNum = 2;
            }
            potionPoison.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);

            StartCoroutine("CanShootSpecial2");
        }

    }
    IEnumerator Move(GameObject sLeft, GameObject sRight, Vector3 LeftFrom, Vector3 LeftTo, Vector3 RightFrom, Vector3 RightTo)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += 5 * Time.deltaTime;
            sLeft.transform.position = Vector3.Lerp(LeftFrom, LeftTo, Mathf.SmoothStep(0f, 1f, t));
            sRight.transform.position = Vector3.Lerp(RightFrom, RightTo, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
    IEnumerator DemonBones(int playerNum, float x, float y)
    {
        yield return new WaitForSeconds(0.05f);


        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        GameObject bigBoneLeft = Instantiate(Resources.Load("Demon_Big_Bone"), new Vector2(player.GetComponent<Rigidbody2D>().position.x - 0.26354105f, player.GetComponent<Rigidbody2D>().position.y - 0.01315464f), Quaternion.identity) as GameObject;
        GameObject bigBoneRight = Instantiate(Resources.Load("Demon_Big_Bone"), new Vector2(player.GetComponent<Rigidbody2D>().position.x + 0.26354105f, player.GetComponent<Rigidbody2D>().position.y - 0.01315464f), Quaternion.identity) as GameObject;
        bigBoneRight.transform.localScale = new Vector3(-1, 1, 1);

        GameObject bigColLeft = Instantiate(Resources.Load("Demon_Big_Col"), new Vector2(x - 0.2700419f, y - 0.47370224f), Quaternion.identity) as GameObject;
        GameObject bigColRight = Instantiate(Resources.Load("Demon_Big_Col"), new Vector2(x + 0.2700419f, y - 0.47370224f), Quaternion.identity) as GameObject;
        bigColRight.transform.localScale = new Vector2(-1, 1);

        Vector3 lFrom = bigColLeft.transform.position;
        Vector3 lTo = bigColLeft.transform.position + new Vector3(0, 0.495f, 0);
        Vector3 rFrom = bigColRight.transform.position;
        Vector3 rTo = bigColRight.transform.position + new Vector3(0, 0.495f, 0);

        StartCoroutine(Move(bigColLeft, bigColRight, lFrom, lTo, rFrom, rTo));

        if (playerNum == 1)
        {
            bigColLeft.tag = "AttPlayer1";
            bigColLeft.layer = 11;
            bigColRight.tag = "AttPlayer1";
            bigColRight.layer = 11;
        }
        else if (playerNum == 2)
        {
            bigColRight.tag = "AttPlayer2";
            bigColRight.layer = 12;
            bigColLeft.tag = "AttPlayer2";
            bigColLeft.layer = 12;
        }

        Destroy(bigColLeft, 0.4f);
        Destroy(bigColRight, 0.4f);
        Destroy(bigBoneLeft, 0.4f);
        Destroy(bigBoneRight, 0.4f);
    }
    public IEnumerator BombBlast(GameObject bomb)
    {
        yield return new WaitForSeconds(1.2f);
        Explode(bomb);
    }
    public void Explode(GameObject bomb)
    {
        StartCoroutine("Explosion", bomb);
    }
    public IEnumerator Explosion(GameObject bomb)
    {
        yield return new WaitForSeconds(0f);
        if (bomb != null)
        {
            Vector2 pos = new Vector2(bomb.transform.position.x, bomb.transform.position.y);


            GameObject blast = Instantiate(Resources.Load("Ninja_Explosion"), new Vector2(pos.x, pos.y + 0.1f), Quaternion.identity) as GameObject;
            blast.tag = bomb.tag;
            blast.layer = bomb.layer;
            Destroy(bomb);

            StartCoroutine("BlastOff", blast);

            audio.Play("Bomb_Explosion", 0, muteSound);

        }

    }
    IEnumerator BlastOff(GameObject blast)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(blast);
    }

    public void LaunchSpecial3(int playerNum) //B + ↓
    {
        StartCoroutine("DelayAnim");
        if (player.name == "Demon(Clone)" && canShootSp3 && !player.dead)
        {
            player.manager.GetComponent<Manager>().coolingDown3 = true;
            Vector2 offsetShield = new Vector2(0.2900102f, 0.02229776f);
            float posShield;

            if (player.isRight)
            {
                direction = 0f;
                posShield = 0.2900102f;
            }
            else
            {
                direction = 180f;
                posShield = -0.2900102f;
            }


            GameObject boneShield = Instantiate(Resources.Load("Demon_Shield"), new Vector2(player.GetComponent<Rigidbody2D>().position.x + posShield, player.GetComponent<Rigidbody2D>().position.y + 0.02229776f), Quaternion.Euler(0, direction, 0)) as GameObject;
            if (playerNum == 1)
            {
                boneShield.tag = "ShieldPlayer1";
                boneShield.layer = 13;
            }
            else if (playerNum == 2)
            {
                boneShield.tag = "ShieldPlayer2";
                boneShield.layer = 14;
            }
            Destroy(boneShield, lifeTime_special3);
            StartCoroutine("CanShootSpecial3");

            audio.Play("Demon_Shield", 0, muteSound);

        }

        if (player.name == "Alien(Clone)" && canShootSp3 && !player.dead && !player.grounded)
        {
            player.manager.GetComponent<Manager>().coolingDown3 = true;

            GameObject UFO = Instantiate(Resources.Load("Alien_UFO"), new Vector2(player.GetComponent<Rigidbody2D>().position.x + 0.019f, player.GetComponent<Rigidbody2D>().position.y - 0.307f), Quaternion.identity) as GameObject;
            if (playerNum == 1)
            {
                UFO.tag = "ShieldPlayer1";
                UFO.layer = 10;
            }
            else if (playerNum == 2)
            {
                UFO.tag = "ShieldPlayer2";
                UFO.layer = 10;
            }
            Destroy(UFO, 2);
            StartCoroutine("CanShootSpecial3");

            audio.Play("Ufo", 0, muteSound);
            audio.Stop("Ufo", 2, muteSound);
        }

        if (player.name == "Ninja(Clone)" && canShootSp3 && !player.dead)
        {
            player.manager.GetComponent<Manager>().coolingDown3 = true;

            Vector2 pos = new Vector2(player.GetComponent<Rigidbody2D>().position.x, player.GetComponent<Rigidbody2D>().position.y);

            GameObject log = Instantiate(Resources.Load("Ninja_Log"), new Vector2(player.GetComponent<Rigidbody2D>().position.x, player.GetComponent<Rigidbody2D>().position.y), Quaternion.identity) as GameObject;
            player.transform.position = pos;
            StartCoroutine("CanShootSpecial3");
            player.GetComponent<Collider2D>().enabled = false;
            player.rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            player.GetComponent<Renderer>().enabled = false;
            StartCoroutine("LogEffect");
            Destroy(log, 1f);

            audio.Play("Log", 0, muteSound);

        }

        if (player.name == "Scientist(Clone)" && canShootSp3 && !player.dead)
        {
            player.manager.GetComponent<Manager>().coolingDown3 = true;

            Vector2 offset;
            Vector2 velocity = new Vector2(1.8f, 2.3f);
            if (player.isRight)
            {
                direction = 0f;
                offset = new Vector2(0.2f, 0.1f);
            }
            else
            {
                direction = 180f;
                offset = new Vector2(0.2f, -0.1f);
            }

            audio.Play("Throw", 0, muteSound);

            GameObject potionSlimeWall = (GameObject)Instantiate(Resources.Load("Scientist_Potion_SlimeWall"), (Vector2)transform.position + offset * transform.localScale.x, Quaternion.Euler(0, direction, 0));
            if (playerNum == 1)
            {
                potionSlimeWall.GetComponent<PotionPoison>().playerNum = 1;
            }
            else if (playerNum == 2)
            {
                potionSlimeWall.GetComponent<PotionPoison>().playerNum = 2;
            }
            potionSlimeWall.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);

            StartCoroutine("CanShootSpecial3");



        }
    }
    IEnumerator LogEffect()
    {
        yield return new WaitForSeconds(1f);

        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<Renderer>().enabled = true;
        rb2d.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        player.maxJump = 2;

        audio.Play("Log", 0, muteSound);

    }

}

