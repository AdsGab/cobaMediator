using System;
using System.Collections.Generic;

namespace MediatorTubesTes
{
    public interface IElectronicsMediator
    {
        void AddElectronic(Electronics electronic);
        void ActivateTrigger(Electronics electronic, Trigger trigger);
    }

    public class ElectronicsMediator : IElectronicsMediator
    {
        private List<Electronics> electronicsList;

        public ElectronicsMediator()
        {
            electronicsList = new List<Electronics>();
        }

        public void AddElectronic(Electronics electronic)
        {
            electronicsList.Add(electronic);
        }

        public void ActivateTrigger(Electronics electronic, Trigger trigger)
        {
            electronic.ActivateTrigger(trigger);
        }
    }

    public enum ElectronicState
    {
        BelumDitambahSmarthome,
        BelumDitambahNonSmarthome,
        TelahDitambahSmarhome,
        TelahDitambahNonSmarthome
    }

    public enum Trigger
    {
        TombolAdd,
        SmarthomeTercentang,
        SmarthomeTidakTercentang
    }

    public class KondisiElectronics
    {
        public ElectronicState StateAwal { get; }
        public ElectronicState StateAkhir { get; }
        public Trigger Trigger { get; }

        public KondisiElectronics(ElectronicState stateAwal, ElectronicState stateAkhir, Trigger trigger)
        {
            StateAwal = stateAwal;
            StateAkhir = stateAkhir;
            Trigger = trigger;
        }
    }

    public class Electronics
    {
        private bool tombolAddTriggered = false;
        public ElectronicsConfig config;
        private IElectronicsMediator mediator;

        public Electronics(ElectronicsConfig config, IElectronicsMediator mediator)
        {
            this.config = config;
            this.mediator = mediator;
        }

        private readonly KondisiElectronics[] transisi =
        {
            new KondisiElectronics(ElectronicState.BelumDitambahNonSmarthome, ElectronicState.BelumDitambahSmarthome, Trigger.SmarthomeTercentang),
            new KondisiElectronics(ElectronicState.BelumDitambahNonSmarthome, ElectronicState.TelahDitambahNonSmarthome, Trigger.TombolAdd),
            new KondisiElectronics(ElectronicState.BelumDitambahSmarthome, ElectronicState.TelahDitambahSmarhome, Trigger.TombolAdd),
            new KondisiElectronics(ElectronicState.BelumDitambahSmarthome, ElectronicState.BelumDitambahNonSmarthome, Trigger.SmarthomeTidakTercentang),
            new KondisiElectronics(ElectronicState.TelahDitambahNonSmarthome, ElectronicState.TelahDitambahSmarhome, Trigger.SmarthomeTercentang),
            new KondisiElectronics(ElectronicState.TelahDitambahSmarhome, ElectronicState.TelahDitambahNonSmarthome, Trigger.SmarthomeTidakTercentang)
        };

        public ElectronicState currentState = ElectronicState.BelumDitambahNonSmarthome;

        public ElectronicState GetNextState(ElectronicState stateAwal, Trigger trigger)
        {
            foreach (var perubahan in transisi)
            {
                if (stateAwal == perubahan.StateAwal && trigger == perubahan.Trigger)
                {
                    return perubahan.StateAkhir;
                }
            }
            return stateAwal;
        }

        public void ActivateTrigger(Trigger trigger)
        {
            if ((trigger == Trigger.SmarthomeTercentang || trigger == Trigger.SmarthomeTidakTercentang) && tombolAddTriggered)
            {
                Console.WriteLine("Tombol Add telah tertekan, anda tidak bisa mengubah status smarthome lagi.");
                return;
            }

            if (trigger == Trigger.TombolAdd)
            {
                tombolAddTriggered = true;
            }
            currentState = GetNextState(currentState, trigger);

            if (currentState == ElectronicState.TelahDitambahNonSmarthome)
            {
                Console.WriteLine($"Perangkat {config.jenis} dengan Nama {config.nama} dan Voltase {config.voltase} berhasil ditambahkan");
            }
            else if (currentState == ElectronicState.TelahDitambahSmarhome)
            {
                Console.WriteLine($"{config.nama} berhasil ditambahkan sebagai perangkat Smarthome");
            }
            else if (currentState == ElectronicState.BelumDitambahNonSmarthome)
            {
                Console.WriteLine($"{config.nama} tidak terhubung ke Smarthome");
                config.isSmarthome = false;
            }
            else if (currentState == ElectronicState.BelumDitambahSmarthome)
            {
                Console.WriteLine($"{config.nama} terhubung ke Smarthome");
                config.isSmarthome = true;
            }
            Console.WriteLine($"Status perangkat ini: {currentState}");
        }
    }

    public class ElectronicsConfig
    {
        public string nama { get; }
        public string jenis { get; }
        public string merk { get; }
        public int voltase { get; }
        public bool isSmarthome { get; set; }

        public ElectronicsConfig(string nama, string jenis, string merk, int voltase, bool isSmarthome)
        {
            this.nama = nama;
            this.jenis = jenis;
            this.merk = merk;
            this.voltase = voltase;
            this.isSmarthome = isSmarthome;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IElectronicsMediator mediator = new ElectronicsMediator();

            ElectronicsConfig config1 = new ElectronicsConfig("Lamp", "Lighting", "Philips", 220, false);
            ElectronicsConfig config2 = new ElectronicsConfig("Thermostat", "Climate Control", "Nest", 110, true);
            ElectronicsConfig config3 = new ElectronicsConfig("Speaker", "Audio", "Sony", 220, false);

            Electronics lamp = new Electronics(config1, mediator);
            Electronics thermostat = new Electronics(config2, mediator);
            Electronics speaker = new Electronics(config3, mediator);

            mediator.AddElectronic(lamp);
            mediator.AddElectronic(thermostat);
            mediator.AddElectronic(speaker);

            lamp.ActivateTrigger(Trigger.TombolAdd);
            thermostat.ActivateTrigger(Trigger.TombolAdd);
            speaker.ActivateTrigger(Trigger.SmarthomeTercentang);

            Console.ReadKey();
        }
    }
}
