using System;
using System.Collections.Generic;

namespace AutoChessConsole
{
    // Интерфейс для коллекции юнитов
    public interface IUnitCollection
    {
        void Add(Unit unit);
        bool Contains(Unit unit);
        List<Unit> GetAliveUnits();
    }

    // Реализация коллекции юнитов
    public class UnitCollection : IUnitCollection
    {
        private List<Unit> units;

        public UnitCollection()
        {
            units = new List<Unit>();
        }

        public void Add(Unit unit)
        {
            units.Add(unit);
        }

        public bool Contains(Unit unit)
        {
            return units.Contains(unit);
        }

        public List<Unit> GetAliveUnits()
        {
            return units.FindAll(unit => unit.IsAlive);
        }
    }

    [Serializable]
    public class GameState
    {
        public IUnitCollection PlayerUnits { get; private set; }
        public IUnitCollection EnemyUnits { get; private set; }
        public int PlayerMoney { get; private set; }

        public GameState()
        {
            PlayerUnits = new UnitCollection();
            EnemyUnits = new UnitCollection();
            PlayerMoney = 250; // Начальные деньги игрока
        }

        public void DeductMoney(int amount)
        {
            if (PlayerMoney >= amount)
            {
                PlayerMoney -= amount;
            }
            else
            {
                throw new InvalidOperationException("Недостаточно денег.");
            }
        }
    }

    [Serializable]
    public class Unit
    {
        public string Name { get; set; } = string.Empty; // Инициализация по умолчанию
        public int Health { get; set; }
        public int Damage { get; set; }
        public bool IsAlive { get; set; }

        public virtual void Attack(Unit target)
        {
            if (IsAlive && target != null) // Проверка на null
            {
                target.TakeDamage(Damage);
                Console.WriteLine($"{Name} атакует {target.Name} на {Damage} урона!");
            }
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                IsAlive = false;
                Console.WriteLine($"{Name} погиб.");
            }
        }

        public override string ToString()
        {
            return $"{Name},{Health},{Damage},{IsAlive}";
        }

        public static Unit FromString(string data)
        {
            var parts = data.Split(',');
            return new Unit
            {
                Name = parts[0],
                Health = int.Parse(parts[1]),
                Damage = int.Parse(parts[2]),
                IsAlive = bool.Parse(parts[3])
            };
        }
    }

    [Serializable]
    public class Dragon : Unit
    {
        public Dragon(string name, int health, int damage)
        {
            Name = name;
            Health = health;
            Damage = damage;
            IsAlive = true;
        }

        public override void Attack(Unit target)
        {
            if (IsAlive && target != null) // Проверка на null
            {
                target.TakeDamage(Damage);
                Console.WriteLine($"{Name} стреляет шаром в {target.Name} на {Damage} урона!");
            }
        }
    }

    public class GameManager
    {
        private const int HeroCost = 50;
        private GameState gameState;
        private List<Unit> availableHeroes = new List<Unit>(); // Инициализация пустым списком

        public GameManager()
        {
            gameState = new GameState();
            InitializeAvailableHeroes();
            InitializeEnemyTeam();
        }
        private void InitializeAvailableHeroes()
        {
            availableHeroes.Add(new Unit { Name = "Воин 1", Health = 100, Damage = 20, IsAlive = true });
            availableHeroes.Add(new Unit { Name = "Воин 2", Health = 120, Damage = 15, IsAlive = true });
            availableHeroes.Add(new Unit { Name = "Воин 3", Health = 90, Damage = 25, IsAlive = true });
            availableHeroes.Add(new Dragon("Дракон 1", 150, 30));
            availableHeroes.Add(new Dragon("Дракон 2", 140, 35));
        }

        private void InitializeEnemyTeam()
        {
            gameState.EnemyUnits.Add(new Unit { Name = "Враг 1", Health = 80, Damage = 18, IsAlive = true });
            gameState.EnemyUnits.Add(new Unit { Name = "Враг 2", Health = 90, Damage = 22, IsAlive = true });
            gameState.EnemyUnits.Add(new Unit { Name = "Враг 3", Health = 110, Damage = 20, IsAlive = true });
            gameState.EnemyUnits.Add(new Dragon("Вражеский Дракон 1", 160, 25));
            gameState.EnemyUnits.Add(new Dragon("Вражеский Дракон 2", 150, 30));
            Console.WriteLine("Команда врага сформирована.");
        }

        public void SelectPlayerTeam()
        {
            Console.WriteLine("Выберите героев для своей команды (введите номера героев, разделенные запятыми, или 'Старт' для начала боя):");
            while (true)
            {
                for (int i = 0; i < availableHeroes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {availableHeroes[i].Name} (Здоровье: {availableHeroes[i].Health}, Урон: {availableHeroes[i].Damage})");
                }

                Console.WriteLine($"У вас {gameState.PlayerMoney} монет. Выбор героя стоит {HeroCost} монет.");
                Console.Write("Ваш выбор: ");
                string input = Console.ReadLine();
                if (input.ToLower() == "старт")
                {
                    if (gameState.PlayerUnits.GetAliveUnits().Count > 0)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Вы должны выбрать хотя бы одного героя перед началом боя.");
                        continue;
                    }
                }

                var choices = input.Split(',');
                foreach (var choice in choices)
                {
                    if (int.TryParse(choice.Trim(), out int index) && index > 0 && index <= availableHeroes.Count)
                    {
                        var selectedHero = availableHeroes[index - 1];
                        if (!gameState.PlayerUnits.Contains(selectedHero))
                        {
                            try
                            {
                                gameState.PlayerUnits.Add(selectedHero);
                                gameState.DeductMoney(HeroCost);
                                Console.WriteLine($"{selectedHero.Name} добавлен в вашу команду. У вас осталось {gameState.PlayerMoney} монет.");
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{selectedHero.Name} уже в вашей команде.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Неверный выбор: {choice}. Пожалуйста, попробуйте снова.");
                    }
                }
            }
        }

        public void StartBattle()
        {
            Console.WriteLine("Бой начался!");
            while (gameState.PlayerUnits.GetAliveUnits().Count > 0 && gameState.EnemyUnits.GetAliveUnits().Count > 0)
            {
                foreach (var playerUnit in gameState.PlayerUnits.GetAliveUnits())
                {
                    var enemyUnit = gameState.EnemyUnits.GetAliveUnits().Find(unit => unit.IsAlive);
                    if (enemyUnit != null)
                    {
                        playerUnit.Attack(enemyUnit);
                    }
                }

                foreach (var enemyUnit in gameState.EnemyUnits.GetAliveUnits())
                {
                    var playerUnit = gameState.PlayerUnits.GetAliveUnits().Find(unit => unit.IsAlive);
                    if (playerUnit != null)
                    {
                        enemyUnit.Attack(playerUnit);
                    }
                }
            }

            if (gameState.PlayerUnits.GetAliveUnits().Count > 0)
            {
                Console.WriteLine("Вы победили!");
            }
            else
            {
                Console.WriteLine("Вы проиграли.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameManager gameManager = new GameManager();
            gameManager.SelectPlayerTeam();
            gameManager.StartBattle();
        }
    }
}







