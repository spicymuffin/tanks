namespace TankStatistics
{
    public class Stats
    {
        public Client client;
        //standart
        public int shots;
        public int closeCalls;
        public int ADTotal;
        public int kills;
        public int deaths;
        //AD specific
        public int shieldBlocks;
        public int landminesCreated;
        public int landmineKills;

        public Stats()
        {
            shots = 0;
            closeCalls = 0;
            ADTotal = 0;
            kills = 0;
            deaths = 0;
            shieldBlocks = 0;
            landminesCreated = 0;
            landmineKills = 0;
        }

        public Stats(int _shots, int _closeCalls, int _ADTotal, int _kills, int _deaths, int _shieldBlocks, int _landminesCreated, int _landmineKills)
        {
            shots = _shots;
            closeCalls = _closeCalls;
            ADTotal = _ADTotal;
            kills = _kills;
            deaths = _deaths;
            shieldBlocks = _shieldBlocks;
            landminesCreated = _landminesCreated;
            landmineKills = _landmineKills;
        }

        public Stats(int _shots, int _closeCalls, int _ADTotal, int _kills, int _deaths, int _shieldBlocks, int _landminesCreated, int _landmineKills, Client _player)
        {
            shots = _shots;
            closeCalls = _closeCalls;
            ADTotal = _ADTotal;
            kills = _kills;
            deaths = _deaths;
            shieldBlocks = _shieldBlocks;
            landminesCreated = _landminesCreated;
            landmineKills = _landmineKills;
            client = _player;
        }
    }
}