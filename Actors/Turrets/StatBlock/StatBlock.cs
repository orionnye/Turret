using Godot;
using System;

public partial class StatBlock : Node
{
	/// <summary>
	/// 	Stats need to be negative as well... that's okay, we'll handle that later one stat at a time
	/// </summary>
	///


	// Stat block
	[Export] public float fireRate = 1f;
	[Export] public float maxGain = 2;
	[Export] public float gainRate = 0.01f;
	[Export] public float dampLock = 30;
	[Export] public float dampPassive = 5;
	[Export] public float aimMargin = 0.2f;

	// block stats...
	public int statTotal = 6;
	public String GetLetters(int radix = 16) {
		String letters = "";
		// Assign the letters
		// but manageable for now, very sad
		letters += "f" +fireRate.ToString();
		letters += "mg" + maxGain.ToString();
		letters += "gr" + gainRate.ToString();
		letters += "dl" + dampLock.ToString();
		letters += "dp" + dampPassive.ToString();
		letters += "am" + aimMargin.ToString();
		return letters;
	}
	public StatBlock Metric() {
		StatBlock metric = new StatBlock{
			// default first bench mark
			fireRate = -1,
			maxGain = 2,
			gainRate = 0.001f,
			dampLock = 30,
			dampPassive = 5,
			aimMargin = 0.2f
		};
		return metric;
	}
	public StatBlock Zero() {
		StatBlock zero = new StatBlock
		{
			fireRate = 0,
			maxGain = 0,
			gainRate = 0,
			dampLock = 0,
			dampPassive = 0,
			aimMargin = 0
		};
		return zero;
	}
	public StatBlock Scale(int sum) {
		float flat = sum/statTotal;
		StatBlock scaled = new StatBlock
		{
			fireRate = flat,
			maxGain = flat,
			gainRate = flat,
			dampLock = flat,
			dampPassive = flat,
			aimMargin = flat
		};
		return scaled;
	}
	public StatBlock Random() {
		// random values between -1 and 1 the nmultiplied by the metric
		// divide all the blocks
		StatBlock metric = new StatBlock().Metric();

		RandomNumberGenerator rng = new RandomNumberGenerator();
		int range = 1;
		float center = -0.2f;
		StatBlock scrambled = new StatBlock
		{
			fireRate = (rng.Randf()*range*2)+center,
			maxGain = (rng.Randf()*range*2)+center,
			gainRate = (rng.Randf()*range*2)+center,
			dampLock = (rng.Randf()*range*2)+center,
			dampPassive = (rng.Randf()*range*2)+center,
			aimMargin = (rng.Randf()*range*2)+center
		};
		return scrambled.Cross(metric);
	}
	public StatBlock Cross(StatBlock term) {
		StatBlock product = new StatBlock
		{
			fireRate = term.fireRate*fireRate,
			maxGain = term.maxGain*maxGain,
			gainRate = term.gainRate*gainRate,
			dampLock = term.dampLock*dampLock,
			dampPassive = term.dampPassive*dampPassive,
			aimMargin = term.aimMargin*aimMargin
		};
		return product;
	}
	public StatBlock Add(StatBlock statblock) {
		StatBlock sum = new StatBlock
		{
			fireRate = statblock.fireRate + fireRate,
			maxGain = statblock.maxGain + maxGain,
			gainRate = statblock.gainRate + gainRate,
			dampLock = statblock.dampLock + dampLock,
			dampPassive = statblock.dampPassive + dampPassive,
			aimMargin = statblock.fireRate + aimMargin
		};
		return sum;
	}

}
