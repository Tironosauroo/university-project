using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using System.Collections;

public class PlayerMovementTest
{
    private GameObject player;
    private PlayerMovement movement;
    private CharacterController controller;
    private GameObject cameraObj;

    [SetUp]
    public void Setup()
    {
        player = new GameObject("Player");
        controller = player.AddComponent<CharacterController>();

        cameraObj = new GameObject("Camera");
        cameraObj.transform.SetParent(player.transform);

        movement = player.AddComponent<PlayerMovement>();
        movement.cameraTransform = cameraObj.transform;


        player.SendMessage("Awake");
        player.SendMessage("Start");
        player.SendMessage("OnEnable");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(player);
    }

    [UnityTest]
    public IEnumerator Player_Moves_Forward_When_Input_Given()
    {
        // arrange
        Vector3 startPos = player.transform.position;

        // forward
        movement.GetType().GetField("moveInput",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, new Vector2(0, 1));

        // act
        for (int i = 0; i < 10; i++)
        {
            player.SendMessage("Update");
            yield return null;
        }

        Assert.Greater(player.transform.position.z, startPos.z, "player should go forward");
    }


    [UnityTest]
    public IEnumerator Player_Crouches_And_Returns_To_Normal()
    {
        float normalY = movement.cameraTransform.localPosition.y;

        // crouch
        movement.GetType().GetMethod("StartCrouch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(movement, null);

        yield return null;
        float crouchedY = movement.cameraTransform.localPosition.y;

        // assert 1
        Assert.Less(crouchedY, normalY, "camera should be a bit down");

        // get up
        movement.GetType().GetMethod("StopCrouch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(movement, null);

        yield return null;
        float resetY = movement.cameraTransform.localPosition.y;

        // assert 2
        Assert.AreEqual(normalY, resetY, 0.01f, "camera should be on normal position");
    }

}
