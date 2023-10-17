//using UnityEngine;
//using UnityEngine.TestTools;
//using NUnit.Framework;

//public class IsometricTests
//{
//    private GameObject testObject;
//    private Isometric isometricScript;

//    [SetUp]
//    public void Setup()
//    {
//        // Cr?er un GameObject de test et attacher le script Isometric
//        testObject = new GameObject();
//        isometricScript = testObject.AddComponent<Isometric>();
//    }

//    [TearDown]
//    public void Teardown()
//    {
//        // D?truire le GameObject de test ? la fin de chaque test
//        Object.DestroyImmediate(testObject);
//    }

//    [Test]
//    public void UpdateTargetAngle_NegativeAngle_ShouldWrapAroundToPositiveAngle()
//    {
//        // Arrange
//        isometricScript.targetAngle = -45f;

//        // Act
//        isometricScript.UpdateTargetAngle();

//        // Assert
//        Assert.AreEqual(315f, isometricScript.targetAngle);
//    }

//    [Test]
//    public void UpdateTargetAngle_AngleGreaterThan360_ShouldWrapAroundToSmallerAngle()
//    {
//        // Arrange
//        isometricScript.targetAngle = 405f;

//        // Act
//        isometricScript.UpdateTargetAngle();

//        // Assert
//        Assert.AreEqual(45f, isometricScript.targetAngle);
//    }

//    [Test]
//    public void UpdateTargetAngle_AngleWithinRange_ShouldRoundToNearestMultipleOf45()
//    {
//        // Arrange
//        isometricScript.targetAngle = 37f;

//        // Act
//        isometricScript.UpdateTargetAngle();

//        // Assert
//        Assert.AreEqual(45f, isometricScript.targetAngle);
//    }
//}