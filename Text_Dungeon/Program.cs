using System;
using System.Numerics;
using System.Reflection.Metadata;
using System.Xml;
using System.IO;

namespace Text_Dungeon
{
    public class Player
    {
        public string name;             // 플레이어 이름
        public int level;                  // 플레이어 레벨
        public string playerClass;      // 플레이어 직업

        public double baseAtk;         // 플레이어 기본 공격력
        public int baseDef;              // 플레이어 기본 방어력

        public double Atk;              // 플레이어가 무기 장착을 통해 얻은 추가 공격력
        public int Def;                   // 플레이어가 방어구 장착을 통해 얻은 추가 방어력
        public int Hp;                    // 플레이어의 체력
        public int gold;                  // 플레이어가 소지한 골드

        public bool isWeaponEquipped;   // 플레이어의 무기 장착 여부
        public bool isArmorEquipped;      // 플레이어의 방어구 장착 여부

        public int dungeonClearCounter;   // 플레이어의 레벨 업 여부를 체크하는 던전 클리어 횟수

        // 플레이어의 현재 공격력
        public double ATK
        {
            get { return baseAtk + Atk; }
            set
            {
                Atk = value;
            }
        }

        // 플레이어의 현재 방어력
        public int DEF
        {
            get { return baseDef + Def; }
            set
            {
                Def = value;
            }
        }

        // 플레이어 상태 보기
        public void showStats()
        {
            Console.WriteLine($"Lv. {level: 00}");
            Console.WriteLine($"{name} ( {playerClass} )");

            if (isWeaponEquipped)   // 무기 장착 시 플레이어 상태
            {
                Console.WriteLine($"공격력 : {baseAtk + Atk} (+{Atk})");
            }
            else
                Console.WriteLine($"공격력 : {baseAtk}");
            if (isArmorEquipped)    // 방어구 장착 시 플레이어 상태
            {
                Console.WriteLine($"방어력 : {baseDef + Def} (+{Def})");
            }
            else
                Console.WriteLine($"방어력 : {baseDef} ");
            Console.WriteLine($"체 력 : {Hp}");
            Console.WriteLine($"Gold : {gold} G\n");
        }

        // 플레이어 레벨 업
        public void LevelUp()
        {
            dungeonClearCounter++;

            if(dungeonClearCounter == level)
            {
                Console.WriteLine("Level Up!\n");

                level++;
                baseAtk += 0.5f;
                baseDef += 1;

                dungeonClearCounter = 0;
            }
        }
    }

    public class Equipment
    {
        public string name;
        public string equipmentStats;  // 무기의 주스탯 ex) 공격력, 방어력
        public int stats;  //무기의 실 스탯
        public string canon;   // 무기 설명
        public int price;       // 무기 가격

        public bool IsOutOfStock = false;       // 플레이어의 구매 여부
        public bool IsEquipped = false;         // 플레이어의 장착 여부
    }


    internal class Program
    {
        static Player player = new Player();
        static Equipment[] equipments = new Equipment[7];
        static string filePath = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            filePath = filePath.Replace("\\bin\\Debug\\net8.0", "");
            // 저장되어 있는 플레이어, 장비 xml 파일 읽기
            GetPlayerXml();
            GetEquipmentXml();

            // 게임 시작
            StartVillage();

            // 게임 종료 후 xml 파일 저장
            SetPlayerXml();
            SetEquipmentXml();
        }

        // 플레이어의 행동 입력 받기
        static int getBehavior()
        {
            int behavior;
            bool isNumeric;

            // 잘못된 입력 시 에러 메세지 출력
            while (true)
            {
                Console.WriteLine("\n원하시는 행동을 입력해주세요.");
                Console.Write(">> ");

                try
                {
                    isNumeric = int.TryParse(Console.ReadLine(), out behavior);     // 입력 받은 값이 정수형 int 값인지 확인해서 맞을 경우 Parse 시켜주기
                    if (!isNumeric) { throw new CheckInputException("잘못된 입력입니다.\n"); }  // 아닐 경우 에러 메시지 출력

                    // 플레이어가 유효 범위의 행동 값 입력 시 while문 종료하고 행동 값 반환
                    if (behavior >= 0 && behavior <= equipments.Length)
                    {
                        break;
                    }
                    else
                    {
                        throw new CheckInputException("잘못된 입력입니다.\n");
                    }
                }
                catch (CheckInputException ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }

            return behavior;
        }


        // 게임 시작
        static void StartVillage()
        {
            // 플레이어가 종료 0 을 선택할 때까지 무한 루프 돌기
            while (true)
            {
                clearConsole();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
                Console.ResetColor();

                Console.WriteLine("1. 상태 보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("4. 던전 입장");
                Console.WriteLine("5. 휴식하기\n");

                Console.WriteLine("0. 종료");

                // 플레이어의 행동 값 받기
                int behavior = getBehavior();

                while (!(behavior >= 0 && behavior <= 5))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ResetColor();
                    behavior = getBehavior();
                }

                if (behavior == 0) return;      // 게임 종료
                else if (behavior == 1)
                {
                    // 플레이어 상태 보기              
                    ShowMyStats();
                }
                else if (behavior == 2)
                {
                    // 인벤토리
                    ShowMyInventory();
                }else if (behavior == 3)
                {
                    // 상점 열기
                    ShowShop();
                }
                else if(behavior == 4)
                {
                    // 던전 입장
                    EntertheDungeon();
                }
                else
                {
                    // 휴식 하기
                    clearConsole();
                    GotoInn();
                }
            }
        }

        // 플레이어 상태 보기
        static public void ShowMyStats()
        {
            clearConsole();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n상태 보기");
            Console.ResetColor();
            Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");
            player.showStats();
            Console.WriteLine("0. 나가기");

            // 사용자가 0을 누르면 함수 종료, StartVillage()로 돌아가기
            while (getBehavior() != 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("잘못된 입력입니다.");
                Console.ResetColor();
            }
        }

        // 인벤토리 열기
        static public void ShowMyInventory()
        {
            while (true)
            {
                clearConsole();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\n인벤토리");

                Console.ResetColor();
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

                Console.WriteLine("[아이템 목록]\n");

                // 현재 소지 중인 장비 불러오기
                // IsOutOfStock = true인 Equipments들을 호출하기
                foreach (var e in equipments)
                {
                    if (e.IsOutOfStock)
                    {
                        Console.WriteLine($"- {(e.IsEquipped ? "[E]" : "")}{e.name,-10}| {e.equipmentStats} +{e.stats} | {e.canon}");
                    }
                }

                Console.WriteLine("\n1. 장착 관리");
                Console.WriteLine("0. 나가기");

                int behavior = getBehavior();

                if (behavior == 0) break;

                while (!(behavior == 1))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ResetColor();

                    behavior = getBehavior();
                }

                // 사용자가 장착 관리 선택 시
                if (behavior == 1)
                {
                    // 장착 관리
                    EquipManagement();
                }
            }
        }

        // 장착 관리
        static public void EquipManagement()
        {
            while (true)
            {
                clearConsole();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\n인벤토리 - 장착 관리");
                Console.ResetColor();
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

                Console.WriteLine("[아이템 목록]\n");

                int i = 0;  // 장비의 실제 equipments 배열에서의 위치
                int j = 0;  // 장착 관리에서 표시되는 번호
                int[] temp = new int[equipments.Length];    
                // 현재 IsOutOfStock = true인 Equipments들을 호출하기
                foreach (var e in equipments)
                {
                    if (e.IsOutOfStock)     // 현재 보유 중인 장비인지 확인
                    {
                        Console.WriteLine($"- {j + 1} {(e.IsEquipped ? "[E]" : "")}{e.name,-10}| {e.equipmentStats} +{e.stats} | {e.canon}");
                        temp[j] = i;    // 장착관리에서 j번째 장비의 equipments의 인덱스 위치 i 저장
                        j++;
                    }
                    i++;
                }



                Console.WriteLine("\n0. 나가기");

                //Console.WriteLine("원하시는 행동을 입력해주세요.");
                //Console.Write(">> ");

                int behavior = getBehavior();

                while (!(behavior >= 0 && behavior <= j))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ResetColor();

                    behavior = getBehavior();
                }

                if (behavior == 0) break;      // 장착 관리 종료

                for (int k = 1; k <= equipments.Length; k++)
                {
                    if (behavior == k)      // 현재 플레이어가 선택한 행동 값 확인하기
                    {
                        // 플레이어가 선택한 장비가 이미 장착 중이라면 해제하기
                        if (equipments[temp[k - 1]].IsEquipped)     
                        {
                            equipments[temp[k - 1]].IsEquipped = false;
                            
                            // 플레이어가 선택한 장비가 무기인지 방어구인지 확인
                            if(equipments[temp[k - 1]].equipmentStats.Equals("공격력"))    // 무기일 경우
                            {
                                player.ATK = 0;                             // 무기 장착으로 상승한 플레이어 공격력 낮추기
                                player.isWeaponEquipped = false;     // 무기 장착 여부 false로 설정
                            }
                            else
                            {
                                player.DEF = 0;                             // 방어구 장착으로 상승한 플레이어 방어력 낮추기
                                player.isArmorEquipped = false;       // 방어구 장착 여부 false로 설정
                            }                              
                        }
                        else
                        {
                            // 현재 이미 장착된 무기가 있는지 확인하기
                            if (equipments[temp[k - 1]].equipmentStats.Equals("공격력"))
                            {
                                // 이미 장착 중인 무기가 있을 경우 장착 해제 및 선택한 무기 장착하기
                                if (player.isWeaponEquipped)
                                {
                                    foreach (var e in equipments)
                                    {
                                        if (e.IsEquipped && e.equipmentStats.Equals("공격력"))
                                        {
                                            e.IsEquipped = false;
                                            player.ATK = 0;
                                        }
                                    }
                                }

                                // 무기 장착
                                player.ATK = equipments[temp[k - 1]].stats;
                                player.isWeaponEquipped = true;
                            }
                            else
                            {
                                // 이미 장착 중인 방어구가 있을 경우 장착 해제 및 선택한 방어구 착용하기
                                if (player.isArmorEquipped)
                                {
                                    foreach (var e in equipments)
                                    {
                                        if (e.IsEquipped && e.equipmentStats.Equals("방어력"))
                                        {
                                            e.IsEquipped = false;
                                            player.DEF = 0;
                                        }
                                    }
                                }

                                // 방어구 장착
                                player.DEF = equipments[temp[k - 1]].stats;
                                player.isArmorEquipped = true;
                            }

                            // 선택한 장비 장착 여부 true로 설정
                            equipments[temp[k - 1]].IsEquipped = true;
                        }
                    }
                }
            }
        }

        // 상점 열기
        static public void ShowShop()
        {
            while (true)
            {
                clearConsole();
                // 상점 불러오기
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n상점");
                Console.ResetColor();
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");

                Console.WriteLine("[보유 골드]");
                Console.WriteLine(player.gold + " G\n");

                Console.WriteLine("[아이템 목록]");

                // 장비 목록 불러오기
                // 현재 IsOutOfStock = true일 경우 "구매완료" 출력하기
                foreach (var e in equipments)
                {
                    Console.WriteLine($"- {e.name,-10}| {e.equipmentStats} +{e.stats} | {e.canon,-25} | {(e.IsOutOfStock ? "구매완료" : e.price + " G")}");
                }

                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("0. 나가기");

                int behavior = getBehavior();

                while (!(behavior == 0 || behavior == 1 || behavior == 2))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ResetColor();
                    behavior = getBehavior();
                }

                if (behavior == 0)
                {
                    break;
                }
                else if(behavior == 1)
                {
                    BuyEquipment();     // 아이템 구매
                }else
                    SellEquipment();    // 아이템 판매
            }
        }

        // 아이템 구매
        static public void BuyEquipment()
        {
            while (true)
            {
                clearConsole();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n상점 - 아이템 구매");
                Console.ResetColor();
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");

                Console.WriteLine("[보유 골드]");
                Console.WriteLine(player.gold + " G\n");    // 보유 중인 골드 출력

                Console.WriteLine("[아이템 목록]");

                // 장비 목록 앞에 숫자 띄우기
                int i = 1;
                foreach (var e in equipments)
                {
                    Console.WriteLine($"- {i} {e.name,-10}| {e.equipmentStats} +{e.stats} | {e.canon,-25} | {(e.IsOutOfStock ? "구매완료" : e.price + " G")}");
                    i++;
                }

                Console.WriteLine("\n0. 나가기");


                int behavior;               

                behavior = getBehavior();

                if(behavior == 0) { break; }

                // 장비 선택하기
                for (int j = 1; j <= equipments.Length; j++)
                {
                    if (behavior == j)
                    {
                        // 만약 선택한 장비를 이미 구매했을 경우
                        if (equipments[j - 1].IsOutOfStock)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("이미 구매한 아이템입니다.");
                            Console.ResetColor();
                        }
                        else
                        {
                            // 구매 가능할 경우
                            if (player.gold >= equipments[j - 1].price)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.WriteLine("구매를 완료했습니다.");
                                Console.ResetColor();
                                player.gold -= equipments[j - 1].price;     // 장비 가격만큼 플레이어 골드 차감

                                equipments[j - 1].IsOutOfStock = true;      // 구매 여부 true로 설정
                            }
                            else
                            {
                                // Gold가 부족할 경우
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Gold가 부족합니다.");
                                Console.ResetColor();
                            }
                        }
                    }
                }
            }
        }

        // 아이템 팔기
        static public void SellEquipment()
        {
            while (true)
            {
                clearConsole();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n상점 - 아이템 판매");
                Console.ResetColor();
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");

                Console.WriteLine("[보유 골드]");
                Console.WriteLine(player.gold + " G\n");

                Console.WriteLine("[아이템 목록]");

                int i = 0, j = 0;
                int[] temp = new int[equipments.Length];
                // 현재 IsOutOfStock = true인 Equipments들을 호출하기
                foreach (var e in equipments)
                {
                    if (e.IsOutOfStock)
                    {
                        Console.WriteLine($"- {j + 1} {(e.IsEquipped ? "[E]" : "")}{e.name,-10}| {e.equipmentStats} +{e.stats} | {e.canon} | {e.price / 100 * 85} G");
                        temp[j] = i;
                        j++;
                    }
                    i++;
                }

                Console.WriteLine("\n0. 나가기");

                int behavior;

                while (true)
                {
                    behavior = getBehavior();

                    if (behavior >= 0 && behavior <= j)
                    {
                        break;
                    }

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ResetColor();
                }

                if (behavior == 0) break;

                // 선택한 장비 확인하기
                for (int k = 1; k <= equipments.Length; k++)
                {
                    if (behavior == k)
                    {
                        // 현재 장착 중인 장비 여부 확인하기
                        if (equipments[temp[k - 1]].IsEquipped)
                        {
                            // 현재 장착 중인 장비일 경우 장비 해제
                            equipments[temp[k - 1]].IsEquipped = false;

                            if (equipments[temp[k - 1]].equipmentStats.Equals("공격력"))
                            {
                                player.ATK = 0;
                                player.isWeaponEquipped = false;
                            }
                            else
                            {
                                player.DEF = 0;
                                player.isArmorEquipped = false;
                            }
                        }

                        equipments[temp[k - 1]].IsOutOfStock = false;
                        player.gold += (equipments[temp[k - 1]].price / 100 * 85);      // 장비 가격의 85퍼센트 만큼 플레이어 골드 추가
                    }
                }
            }
        }

        static public void EntertheDungeon()
        {
            // 던전 입장
            clearConsole();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("던전 입장");
            Console.ResetColor();

            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");

            Console.WriteLine("1. 쉬운 던전     | 방어력 5 이상 권장");
            Console.WriteLine("2. 일반 던전     | 방어력 11 이상 권장");
            Console.WriteLine("3. 어려운 던전     | 방어력 17 이상 권장");
            Console.WriteLine("0. 나가기");

            int behavior;

            while (true)
            {
                behavior = getBehavior();

                if (!(behavior >= 0 && behavior <= 3))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ResetColor();
                }
                else break;
            }

            if (behavior != 0)
            {
                // 던전 선택
                SelectDungeon(behavior);
            }
        }

        static public void SelectDungeon(int difficulty)
        {
            string dungeonName = "";
            int recommendedDef = 0;
            int clearGold = 0;

            switch (difficulty)
            {
                case 1:
                    dungeonName = "쉬운";
                    clearGold = 1000;
                    recommendedDef = 5;
                    break;
                case 2:
                    dungeonName = "일반";
                    clearGold = 1700;
                    recommendedDef = 11;
                    break;
                case 3:
                    dungeonName = "어려운";
                    clearGold = 2500;
                    recommendedDef = 17;
                    break;
            }

            // 선택한 던전 입장 결과 출력하기
            DungeonResult(dungeonName, clearGold, recommendedDef);
        }

        // 던전 입장 결과
        static public void DungeonResult(string dungeonName, int clearGold, int recommendedDef)
        {
            clearConsole();
            Random random = new Random();

            bool result = true;
            int loseHp = 0;
            int bonusGold = 0;
            int behavior;

            // 플레이어 방어력이 요구 방어력보다 적을 경우
            if (player.DEF < recommendedDef)
            {
                int chance = random.Next(0, 101);       // chance를 0 ~ 100 사이의 무작위 난수로 만든 후, 40보다 작을 경우 던전 실패

                if (chance <= 40)   // 40퍼센트 확률로 던전 실패하기
                {
                    result = false;
                    loseHp = player.Hp / 2;
                    bonusGold = 0;

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("던전 실패");
                    Console.ResetColor();
                    Console.WriteLine($"{dungeonName} 던전을 클리어 실패 하였습니다.\n");
                }
            }

            // 던전 클리어 시
            if (result)
            {
                // 잃은 체력 = (20 - (플레이어 방어력 - 요구 방어력)) ~ (35 - (플레이어 방어력 - 요구 방어력)) 사이에서 랜덤하게 생성
                loseHp = random.Next(20 - (player.DEF - recommendedDef), 35 - (player.DEF - recommendedDef) + 1);

                // 획득한 골드 = 기본 보상 + 기본 보상의 (플레이어 공격력 ~ 플레이어 공격력 * 2)퍼센트
                bonusGold = clearGold + clearGold / 100 * random.Next((int)player.ATK, (int)player.ATK * 2 + 1);

                // dungeonClearCounter++ 시켜서 레벨 업 여부 확인
                player.LevelUp();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("던전 클리어");
                Console.ResetColor();
                Console.WriteLine("축하합니다!!");
                Console.WriteLine($"{dungeonName} 던전을 클리어 하였습니다.\n");
            }

            Console.WriteLine("[탐험 결과]");
            Console.WriteLine($"체력 {player.Hp} -> {player.Hp - loseHp}");
            Console.WriteLine($"Gold {player.gold} G -> {player.gold + bonusGold} G\n");

            Console.WriteLine("0. 나가기");

            behavior = getBehavior();

            while (behavior != 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("잘못된 입력입니다.");
                Console.ResetColor();
                behavior = getBehavior();
            }

            // 플레이어가 잃은 체력과 얻은 골드 반영하기
            player.Hp -= loseHp;
            player.gold += bonusGold;
        }

        // 휴식하기
        static public void GotoInn()
        {
            clearConsole();
            Console.WriteLine("휴식하기");
            Console.WriteLine($"500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.gold} G)\n");

            Console.WriteLine("1. 휴식하기");
            Console.WriteLine("0. 나가기");

            int behavior = getBehavior();

            while (!(behavior == 0 || behavior == 1))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("잘못된 입력입니다.");
                Console.ResetColor();
                behavior = getBehavior();
            }

            if (behavior == 1 && player.gold >= 500)
            {
                player.gold -= 500;
                player.Hp = 100;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("휴식을 완료했습니다.");
                Console.ResetColor();
            }
            // 플레이어 골드가 500골드 이하일 경우
            else if (behavior == 1 && player.gold < 500)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Gold 가 부족합니다.");
                Console.ResetColor();
            }
        }

        // 장비 xml 파일 읽기
        static public void GetEquipmentXml()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filePath +"\\Equipment.xml");      // Text_Dungeon.exe 기준 xml의 상대 경로. exe파일과 같은 위치에 xml 파일이 있을 경우 읽을 수 있다.

            XmlNodeList nodes = xdoc.SelectNodes("/Item/Equipment");    // foreach문을 돌릴 노드 선택하기

            int i = 0;
            foreach (XmlNode e in nodes)
            {
                equipments[i] = new Equipment();        // equipments[i] 배열 생성
                equipments[i].name = e.SelectSingleNode("name").InnerText;
                equipments[i].equipmentStats = e.SelectSingleNode("equipmentStats").InnerText;
                equipments[i].stats = int.Parse(e.SelectSingleNode("stats").InnerText);
                equipments[i].canon = e.SelectSingleNode("canon").InnerText;
                equipments[i].price = int.Parse(e.SelectSingleNode("price").InnerText);
                equipments[i].IsOutOfStock = bool.Parse(e.SelectSingleNode("IsOutOfStock").InnerText);
                equipments[i].IsEquipped = bool.Parse(e.SelectSingleNode("IsEquipped").InnerText);

                i++;
            }
        }

        // 플레이어 xml 파일 읽기
        static public void GetPlayerXml()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filePath + "\\Player.xml");     // Text_Dungeon.exe 기준 xml의 상대 경로. exe파일과 같은 위치에 xml 파일이 있을 경우 읽을 수 있다.

            XmlNodeList nodes = xdoc.GetElementsByTagName("Player");    // foreach문 돌릴 노드 선택

            foreach (XmlNode e in nodes)
            {
                player.name = e.SelectSingleNode("name").InnerText;
                player.level = int.Parse(e.SelectSingleNode("level").InnerText);
                player.playerClass = e.SelectSingleNode("playerClass").InnerText;
                player.baseAtk = double.Parse(e.SelectSingleNode("baseAtk").InnerText);
                player.baseDef = int.Parse(e.SelectSingleNode("baseDef").InnerText);
                player.Atk = double.Parse(e.SelectSingleNode("Atk").InnerText);
                player.Def = int.Parse(e.SelectSingleNode("Def").InnerText);
                player.Hp = int.Parse(e.SelectSingleNode("Hp").InnerText);
                player.gold = int.Parse(e.SelectSingleNode("gold").InnerText);
                player.isWeaponEquipped = bool.Parse(e.SelectSingleNode("isWeaponEquipped").InnerText);
                player.isArmorEquipped = bool.Parse(e.SelectSingleNode("isArmorEquipped").InnerText);
                player.dungeonClearCounter = int.Parse(e.SelectSingleNode("dungeonClearCounter").InnerText);
            }
        }

        // 플레이어 xml 파일 저장하기
        static public void SetPlayerXml()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filePath + "\\Player.xml");     // Text_Dungeon.exe 기준 xml의 상대 경로. exe파일과 같은 위치에 xml 파일이 있을 경우 읽을 수 있다.

            XmlNodeList nodes = xdoc.GetElementsByTagName("Player");

            foreach (XmlNode e in nodes)
            {
                e.SelectSingleNode("level").InnerText = player.level.ToString();
                e.SelectSingleNode("baseAtk").InnerText = player.baseAtk.ToString();
                e.SelectSingleNode("baseDef").InnerText = player.baseDef.ToString();
                e.SelectSingleNode("Atk").InnerText = player.Atk.ToString();
                e.SelectSingleNode("Def").InnerText = player.Def.ToString();
                e.SelectSingleNode("Hp").InnerText = player.Hp.ToString();
                e.SelectSingleNode("gold").InnerText = player.gold.ToString();
                e.SelectSingleNode("isWeaponEquipped").InnerText = player.isWeaponEquipped.ToString();
                e.SelectSingleNode("isArmorEquipped").InnerText = player.isArmorEquipped.ToString();
                e.SelectSingleNode("dungeonClearCounter").InnerText = player.dungeonClearCounter.ToString();
            }

            xdoc.Save(filePath + "\\Player.xml");     // 플레이어 xml 파일 저장
        }

        // 장비 xml 파일 저장하기
        static public void SetEquipmentXml()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filePath + "\\Equipment.xml");      // Text_Dungeon.exe 기준 xml의 상대 경로. exe파일과 같은 위치에 xml 파일이 있을 경우 읽을 수 있다.

            XmlNodeList nodes = xdoc.SelectNodes("/Item/Equipment");

            int i = 0;
            foreach (XmlNode e in nodes)
            {
                e.SelectSingleNode("name").InnerText = equipments[i].name;
                e.SelectSingleNode("equipmentStats").InnerText = equipments[i].equipmentStats;
                e.SelectSingleNode("stats").InnerText = equipments[i].stats.ToString();
                e.SelectSingleNode("canon").InnerText = equipments[i].canon;
                e.SelectSingleNode("price").InnerText = equipments[i].price.ToString();
                e.SelectSingleNode("IsOutOfStock").InnerText = equipments[i].IsOutOfStock.ToString();
                e.SelectSingleNode("IsEquipped").InnerText = equipments[i].IsEquipped.ToString();

                i++;
            }

            xdoc.Save(filePath + "\\Equipment.xml");      // 장비 xml 파일 저장
        }

        static public void clearConsole()
        {
            Console.Clear();
        }

        // 플레이어 행동 값을 입력 받을 때, 유효 범위 밖을 입력할 경우 에러 메시지 출력하기
        public class CheckInputException : Exception
        {
            public CheckInputException(string message) : base(message)
            {

            }
        }
    }
}
