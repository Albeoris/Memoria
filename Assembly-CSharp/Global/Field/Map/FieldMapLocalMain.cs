using System;
using UnityEngine;

internal class FieldMapLocalMain : HonoBehavior
{
	public void Start()
	{
		FF9StateSystem.Field.twist = Vector2.zero;
	}

	public void Update()
	{
		if (!this.firstTime)
		{
			return;
		}
		this.firstTime = false;
		FieldMap component = GameObject.Find("FieldMap").GetComponent<FieldMap>();
		component.AddPlayer();
		Actor actor = new Actor();
		actor.idle = 200;
		actor.walk = 190;
		actor.run = 38;
		actor.turnl = 187;
		actor.turnr = 193;
		GameObject gameObject = GameObject.Find("Player");
		FieldMapActorController component2 = gameObject.GetComponent<FieldMapActorController>();
		AnimationFactory.AddAnimWithAnimatioName(gameObject, FF9DBAll.AnimationDB.GetValue((Int32)actor.idle));
		AnimationFactory.AddAnimWithAnimatioName(gameObject, FF9DBAll.AnimationDB.GetValue((Int32)actor.walk));
		AnimationFactory.AddAnimWithAnimatioName(gameObject, FF9DBAll.AnimationDB.GetValue((Int32)actor.run));
		AnimationFactory.AddAnimWithAnimatioName(gameObject, FF9DBAll.AnimationDB.GetValue((Int32)actor.turnl));
		AnimationFactory.AddAnimWithAnimatioName(gameObject, FF9DBAll.AnimationDB.GetValue((Int32)actor.turnr));
		gameObject.GetComponent<Animation>().Play(FF9DBAll.AnimationDB.GetValue((Int32)actor.idle));
		component2.originalActor = actor;
	}

	private Boolean firstTime = true;
}
