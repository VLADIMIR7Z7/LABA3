using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AutoChessConsole.Tests
{
    [TestClass]
    public class GameStateTests
    {
        private GameState gameState;

        [TestInitialize]
        public void Setup()
        {
            gameState = new GameState();
        }

        [TestMethod]
        public void ResetMoney_ShouldSetPlayerMoneyTo250()
        {
            gameState.ResetMoney();
            Assert.AreEqual(250, gameState.PlayerMoney);
        }

        [TestMethod]
        public void DeductMoney_ShouldDecreasePlayerMoney()
        {
            gameState.DeductMoney(50);
            Assert.AreEqual(200, gameState.PlayerMoney);
        }

        [TestMethod]
        public void DeductMoney_ShouldThrowException_WhenNotEnoughMoney()
        {
            gameState.DeductMoney(250);
            Assert.ThrowsException<InvalidOperationException>(() => gameState.DeductMoney(1));
        }
    }

    [TestClass]
    public class UnitCollectionTests
    {
        private UnitCollection unitCollection;

        [TestInitialize]
        public void Setup()
        {
            unitCollection = new UnitCollection();
        }

        [TestMethod]
        public void Add_ShouldAddUnitToCollection()
        {
            var unit = new Unit { Name = "Warrior", Health = 100, Damage = 20, IsAlive = true };
            unitCollection.Add(unit);
            Assert.IsTrue(unitCollection.Contains(unit));
        }

        [TestMethod]
        public void GetAliveUnits_ShouldReturnOnlyAliveUnits()
        {
            var unit1 = new Unit { Name = "Warrior 1", Health = 100, Damage = 20, IsAlive = true };
            var unit2 = new Unit { Name = "Warrior 2", Health = 0, Damage = 20, IsAlive = false };
            unitCollection.Add(unit1);
            unitCollection.Add(unit2);

            var aliveUnits = unitCollection.GetAliveUnits();
            Assert.AreEqual(1, aliveUnits.Count);
            Assert.AreEqual(unit1, aliveUnits[0]);
        }
    }
}