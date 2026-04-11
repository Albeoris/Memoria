using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Global.Sound.SaXAudio
{
    public sealed class EffectPreset : ICsvEntry
    {
        public enum Effect : Byte
        {
            None = 0,
            Reverb = 1,
            Eq = 1 << 1,
            Echo = 1 << 2,
            Volume = 1 << 3,
            All = Reverb | Eq | Echo | Volume
        }

        public enum Layer : Byte
        {
            None = 0,
            Music = 1,
            Ambient = 1 << 1,
            SoundEffect = 1 << 2,
            Voice = 1 << 3,
            All = Music | Ambient | SoundEffect | Voice
        }

        public String Name = "";
        public Effect Effects = Effect.None;
        public SaXAudio.ReverbParameters Reverb = new SaXAudio.ReverbParameters();
        public SaXAudio.EqParameters Eq = new SaXAudio.EqParameters();
        public SaXAudio.EchoParameters Echo = new SaXAudio.EchoParameters();
        public Single Volume = 1f;

        public Layer Layers = Layer.None;
        public HashSet<Int32> FieldIDs = new HashSet<Int32>();
        public HashSet<Int32> BattleIDs = new HashSet<Int32>();
        public HashSet<Int32> BattleBgIDs = new HashSet<Int32>();
        public HashSet<String> ResourceIDs = new HashSet<String>();
        public String Condition = "";

        public EffectPreset() { }

        public Boolean IsBusInLayers(Int32 bus)
        {
            if (!AudioEffectManager.IsSaXAudio || Layers == Layer.None)
                return false;

            SdLibAPIWithSaXAudio saXAudio = ISdLibAPIProxy.Instance as SdLibAPIWithSaXAudio;
            if (saXAudio == null) return false;

            if ((Layers & Layer.Music) != 0 && bus == saXAudio.BusMusic)
                return true;
            if ((Layers & Layer.Ambient) != 0 && bus == saXAudio.BusAmbient)
                return true;
            if ((Layers & Layer.SoundEffect) != 0 && bus == saXAudio.BusSoundEffect)
                return true;
            if ((Layers & Layer.Voice) != 0 && bus == saXAudio.BusVoice)
                return true;
            return false;
        }

        public void RemoveIDs()
        {
            FieldIDs = null;
            BattleIDs = null;
            BattleBgIDs = null;
            ResourceIDs = null;
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            if (raw.Length < 47) return;

            Name = raw[0];
            Effects = (Effect)Byte.Parse(raw[1]);

            Reverb.WetDryMix = Single.Parse(raw[2], CultureInfo.InvariantCulture);
            Reverb.ReflectionsDelay = UInt32.Parse(raw[3], CultureInfo.InvariantCulture);
            Reverb.ReverbDelay = Byte.Parse(raw[4], CultureInfo.InvariantCulture);
            Reverb.RearDelay = Byte.Parse(raw[5], CultureInfo.InvariantCulture);
            Reverb.SideDelay = Byte.Parse(raw[6], CultureInfo.InvariantCulture);
            Reverb.PositionLeft = Byte.Parse(raw[7], CultureInfo.InvariantCulture);
            Reverb.PositionRight = Byte.Parse(raw[8], CultureInfo.InvariantCulture);
            Reverb.PositionMatrixLeft = Byte.Parse(raw[9], CultureInfo.InvariantCulture);
            Reverb.PositionMatrixRight = Byte.Parse(raw[10], CultureInfo.InvariantCulture);
            Reverb.EarlyDiffusion = Byte.Parse(raw[11], CultureInfo.InvariantCulture);
            Reverb.LateDiffusion = Byte.Parse(raw[12], CultureInfo.InvariantCulture);
            Reverb.LowEQGain = Byte.Parse(raw[13], CultureInfo.InvariantCulture);
            Reverb.LowEQCutoff = Byte.Parse(raw[14], CultureInfo.InvariantCulture);
            Reverb.HighEQGain = Byte.Parse(raw[15], CultureInfo.InvariantCulture);
            Reverb.HighEQCutoff = Byte.Parse(raw[16], CultureInfo.InvariantCulture);
            Reverb.RoomFilterFreq = Single.Parse(raw[17], CultureInfo.InvariantCulture);
            Reverb.RoomFilterMain = Single.Parse(raw[18], CultureInfo.InvariantCulture);
            Reverb.RoomFilterHF = Single.Parse(raw[19], CultureInfo.InvariantCulture);
            Reverb.ReflectionsGain = Single.Parse(raw[20], CultureInfo.InvariantCulture);
            Reverb.ReverbGain = Single.Parse(raw[21], CultureInfo.InvariantCulture);
            Reverb.DecayTime = Single.Parse(raw[22], CultureInfo.InvariantCulture);
            Reverb.Density = Single.Parse(raw[23], CultureInfo.InvariantCulture);
            Reverb.RoomSize = Single.Parse(raw[24], CultureInfo.InvariantCulture);
            Reverb.DisableLateField = Boolean.Parse(raw[25]);

            Eq.FrequencyCenter0 = Single.Parse(raw[26], CultureInfo.InvariantCulture);
            Eq.Gain0 = Single.Parse(raw[27], CultureInfo.InvariantCulture);
            Eq.Bandwidth0 = Single.Parse(raw[28], CultureInfo.InvariantCulture);
            Eq.FrequencyCenter1 = Single.Parse(raw[29], CultureInfo.InvariantCulture);
            Eq.Gain1 = Single.Parse(raw[30], CultureInfo.InvariantCulture);
            Eq.Bandwidth1 = Single.Parse(raw[31], CultureInfo.InvariantCulture);
            Eq.FrequencyCenter2 = Single.Parse(raw[32], CultureInfo.InvariantCulture);
            Eq.Gain2 = Single.Parse(raw[33], CultureInfo.InvariantCulture);
            Eq.Bandwidth2 = Single.Parse(raw[34], CultureInfo.InvariantCulture);
            Eq.FrequencyCenter3 = Single.Parse(raw[35], CultureInfo.InvariantCulture);
            Eq.Gain3 = Single.Parse(raw[36], CultureInfo.InvariantCulture);
            Eq.Bandwidth3 = Single.Parse(raw[37], CultureInfo.InvariantCulture);

            Echo.WetDryMix = Single.Parse(raw[38], CultureInfo.InvariantCulture);
            Echo.Feedback = Single.Parse(raw[39], CultureInfo.InvariantCulture);
            Echo.Delay = Single.Parse(raw[40], CultureInfo.InvariantCulture);

            Volume = Single.Parse(raw[41], CultureInfo.InvariantCulture);
            Layers = (Layer)Byte.Parse(raw[42]);

            String[] ids;

            ids = raw[43].Split('|');
            if (ids.Length > 0)
            {
                FieldIDs.Clear();
                foreach (String id in ids)
                    if (id.Length > 0) FieldIDs.Add(Int32.Parse(id));
            }

            ids = raw[44].Split('|');
            if (ids.Length > 0)
            {
                BattleIDs.Clear();
                foreach (String id in ids)
                    if (id.Length > 0) BattleIDs.Add(Int32.Parse(id));
            }

            ids = raw[45].Split('|');
            if (ids.Length > 0)
            {
                BattleBgIDs.Clear();
                foreach (String id in ids)
                    if (id.Length > 0) BattleBgIDs.Add(Int32.Parse(id));
            }

            ids = raw[46].Split('|');
            if (ids.Length > 0)
            {
                ResourceIDs.Clear();
                foreach (String id in ids)
                    if (id.Length > 0) ResourceIDs.Add(id);
            }

            Condition = raw[47];
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.String(Name);
            writer.Byte((Byte)Effects);

            // Reverb
            writer.String(Reverb.WetDryMix.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.ReflectionsDelay.ToString(CultureInfo.InvariantCulture));
            writer.Byte(Reverb.ReverbDelay);
            writer.Byte(Reverb.RearDelay);
            writer.Byte(Reverb.SideDelay);
            writer.Byte(Reverb.PositionLeft);
            writer.Byte(Reverb.PositionRight);
            writer.Byte(Reverb.PositionMatrixLeft);
            writer.Byte(Reverb.PositionMatrixRight);
            writer.Byte(Reverb.EarlyDiffusion);
            writer.Byte(Reverb.LateDiffusion);
            writer.Byte(Reverb.LowEQGain);
            writer.Byte(Reverb.LowEQCutoff);
            writer.Byte(Reverb.HighEQGain);
            writer.Byte(Reverb.HighEQCutoff);
            writer.String(Reverb.RoomFilterFreq.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.RoomFilterMain.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.RoomFilterHF.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.ReflectionsGain.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.ReverbGain.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.DecayTime.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.Density.ToString(CultureInfo.InvariantCulture));
            writer.String(Reverb.RoomSize.ToString(CultureInfo.InvariantCulture));
            writer.Boolean(Reverb.DisableLateField);

            // EQ
            writer.String(Eq.FrequencyCenter0.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Gain0.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Bandwidth0.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.FrequencyCenter1.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Gain1.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Bandwidth1.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.FrequencyCenter2.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Gain2.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Bandwidth2.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.FrequencyCenter3.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Gain3.ToString(CultureInfo.InvariantCulture));
            writer.String(Eq.Bandwidth3.ToString(CultureInfo.InvariantCulture));

            // Echo
            writer.String(Echo.WetDryMix.ToString(CultureInfo.InvariantCulture));
            writer.String(Echo.Feedback.ToString(CultureInfo.InvariantCulture));
            writer.String(Echo.Delay.ToString(CultureInfo.InvariantCulture));

            // Volume & Layers
            writer.String(Volume.ToString(CultureInfo.InvariantCulture));
            writer.Byte((Byte)Layers);

            writer.String(String.Join("|", FieldIDs.Select(x => x.ToString()).ToArray()));
            writer.String(String.Join("|", BattleIDs.Select(x => x.ToString()).ToArray()));
            writer.String(String.Join("|", BattleBgIDs.Select(x => x.ToString()).ToArray()));
            writer.String(String.Join("|", ResourceIDs.ToArray()));

            writer.String("");
        }

        public override string ToString()
        {
            List<String> raw = new List<String>();

            raw.Add(Name);
            raw.Add(((Byte)Effects).ToString());

            raw.Add(Reverb.WetDryMix.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.ReflectionsDelay.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.ReverbDelay.ToString());
            raw.Add(Reverb.RearDelay.ToString());
            raw.Add(Reverb.SideDelay.ToString());
            raw.Add(Reverb.PositionLeft.ToString());
            raw.Add(Reverb.PositionRight.ToString());
            raw.Add(Reverb.PositionMatrixLeft.ToString());
            raw.Add(Reverb.PositionMatrixRight.ToString());
            raw.Add(Reverb.EarlyDiffusion.ToString());
            raw.Add(Reverb.LateDiffusion.ToString());
            raw.Add(Reverb.LowEQGain.ToString());
            raw.Add(Reverb.LowEQCutoff.ToString());
            raw.Add(Reverb.HighEQGain.ToString());
            raw.Add(Reverb.HighEQCutoff.ToString());
            raw.Add(Reverb.RoomFilterFreq.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.RoomFilterMain.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.RoomFilterHF.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.ReflectionsGain.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.ReverbGain.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.DecayTime.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.Density.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.RoomSize.ToString(CultureInfo.InvariantCulture));
            raw.Add(Reverb.DisableLateField.ToString());

            raw.Add(Eq.FrequencyCenter0.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Gain0.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Bandwidth0.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.FrequencyCenter1.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Gain1.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Bandwidth1.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.FrequencyCenter2.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Gain2.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Bandwidth2.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.FrequencyCenter3.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Gain3.ToString(CultureInfo.InvariantCulture));
            raw.Add(Eq.Bandwidth3.ToString(CultureInfo.InvariantCulture));

            raw.Add(Echo.WetDryMix.ToString(CultureInfo.InvariantCulture));
            raw.Add(Echo.Feedback.ToString(CultureInfo.InvariantCulture));
            raw.Add(Echo.Delay.ToString(CultureInfo.InvariantCulture));

            raw.Add(Volume.ToString(CultureInfo.InvariantCulture));
            raw.Add(((Byte)Layers).ToString());

            raw.Add(String.Join("|", FieldIDs.Select(x => x.ToString()).ToArray()));
            raw.Add(String.Join("|", BattleIDs.Select(x => x.ToString()).ToArray()));
            raw.Add(String.Join("|", BattleBgIDs.Select(x => x.ToString()).ToArray()));
            raw.Add(String.Join("|", ResourceIDs.ToArray()));
            raw.Add(Condition);

            return String.Join(";", raw.ToArray());
        }
    }
}
