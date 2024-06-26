using System;
using System.Linq;
using Memoria.Prime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Memoria
{
    public sealed class CharacterBuilder
    {
        private readonly EventEngine _engine;

        public CharacterBuilder(EventEngine engine)
        {
            _engine = engine;
        }

        public void Spawn(ICharacterDescriptor desc)
        {
            try
            {
                Int32 characterId = CreateEmptyEntry();

                Actor actor = CreateActor(desc, characterId);
                CreateModel(actor, desc);
                CreateCharacter(actor);
                CreateState(actor);
                SetActorPosition(actor, desc);
                SetActorAnimation(actor, desc);
                SetActorRadius(actor);

                SpawnActor(actor);

                Log.Message($"The custom character [{desc.Name}] appeared.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to spawn the custom character: [{desc?.Name}]");
            }
        }

        private void SetActorRadius(Actor actor)
        {
            if (_engine.gMode != 1)
                return;

            FieldMapActorController fieldMapActorController = actor.go.GetComponent<FieldMapActorController>();
            if (fieldMapActorController != null)
                fieldMapActorController.radius = 20 * 4;

            FieldMapActor fieldMapActor = actor.go.GetComponent<FieldMapActor>();
            if (fieldMapActor != null)
                fieldMapActor.CharRadius = 20 * 4;
        }

        private Int32 CreateEmptyEntry()
        {
            Int32 characterId = _engine.sObjTable.Length;

            Array.Resize(ref _engine.sObjTable, ++_engine.sSourceObjN);
            ObjTable prevObjTable = _engine.sObjTable.Last(l => l != null);

            _engine.sObjTable[_engine.sObjTable.Length - 1] = new ObjTable
            {
                ofs = prevObjTable.ofs,
                size = prevObjTable.size,
                varn = prevObjTable.varn,
                flags = prevObjTable.flags,
                pad1 = prevObjTable.pad1,
                pad2 = prevObjTable.pad2
            };

            Array.Resize(ref _engine.allObjsEBData, _engine.allObjsEBData.Length + 1);
            _engine.allObjsEBData[_engine.allObjsEBData.Length - 1] = new Byte[0];

            return characterId;
        }

        private void CreateModel(Actor actor, ICharacterDescriptor desc)
        {
            UInt16 modelId = desc.Model;
            String modelName = FF9BattleDB.GEO.GetValue(modelId);

            GameObject modelObject = ModelFactory.CreateModel(modelName);
            modelObject.name = "obj" + actor.uid;

            actor.model = modelId;
            actor.go = modelObject;
            GeoTexAnim.addTexAnim(actor.go, modelName);

            actor.meshIsRendering = CreateMeshRenderingState(modelObject);
        }

        private static Boolean[] CreateMeshRenderingState(GameObject modelObject)
        {
            Int32 length = modelObject.transform.Cast<Object>().Count(child => child.name.Contains("mesh"));

            Boolean[] meshIsRendering = new Boolean[length];
            for (Int32 i = 0; i < length; ++i)
                meshIsRendering[i] = true;

            return meshIsRendering;
        }

        private static Actor CreateActor(ICharacterDescriptor desc, Int32 characterId)
        {
            return new Actor(characterId, characterId, EventEngine.sizeOfActor)
            {
                flags = (Byte)desc.ObjectFlags,
                actf = (UInt16)desc.ActorFlags,
                eye = desc.Eyes,
                collRad = desc.TrackingRadius,
                talkRad = desc.TalkingRadius,
                state = EventEngine.stateRunning,
                neckMyID = desc.NeckMyId,
                neckTargetID = desc.NeckTargetId
            };
        }

        private static void CreateCharacter(Actor actor)
        {
            FF9Char character = new FF9Char
            {
                geo = actor.go,
                evt = actor
            };

            FF9StateSystem.Common.FF9.charArray.Add(actor.uid, character);
        }

        private static void CreateState(Actor actor)
        {
            FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Add(actor.uid, new FF9FieldCharState());
            FF9StateSystem.Field.FF9Field.loc.map.shadowArray.Add(actor.uid, new FF9Shadow());
        }

        private void SetActorPosition(Actor actor, ICharacterDescriptor desc)
        {
            Vector3 position = desc.StartupLocation;
            _engine.SetActorPosition(actor, position.x, position.y, position.z);
            _engine.clrdist(actor);
        }

        private void SetActorAnimation(Actor actor, ICharacterDescriptor desc)
        {
            CharacterAnimation animation = desc.GetAnimation();

            actor.idleSpeed = animation.IdleSpeed;
            actor.idle = animation.Idle;
            actor.walk = animation.Walk;
            actor.run = animation.Run;
            actor.turnl = animation.TurnLeft;
            actor.turnr = animation.TurnRight;

            GameObject model = actor.go;
            AnimationFactory.AddAnimWithAnimatioName(model, FF9DBAll.AnimationDB.GetValue(actor.idle));
            AnimationFactory.AddAnimWithAnimatioName(model, FF9DBAll.AnimationDB.GetValue(actor.walk));
            AnimationFactory.AddAnimWithAnimatioName(model, FF9DBAll.AnimationDB.GetValue(actor.run));
            AnimationFactory.AddAnimWithAnimatioName(model, FF9DBAll.AnimationDB.GetValue(actor.turnl));
            AnimationFactory.AddAnimWithAnimatioName(model, FF9DBAll.AnimationDB.GetValue(actor.turnr));
        }

        private void SpawnActor(Actor actor)
        {
            _engine.fieldmap.AddFieldChar(actor.go, actor.posField, actor.rotField, false, actor, needRestore: false);
        }
    }
}