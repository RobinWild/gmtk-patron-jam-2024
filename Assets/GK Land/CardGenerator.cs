public static class CardGenerator
{
    public static void Taxation(){
        var card = GameManager.instance.slots.NewCard();

        card.RegisterEffects(
            new ResourceGain("gold", 3),
            new TimeMult(5)
        );

        card.textComponent.text = "Taxation";
    }

    public static void WoodSlaves(){
        var card = GameManager.instance.slots.NewCard();

        card.RegisterEffects(
            new ResourceGain("wood", 3),
            new TimeMult(10)
        );

        card.textComponent.text = "Wood\n(slave labour)";
    }

    public static void WoodImport(){
        var card = GameManager.instance.slots.NewCard();

        card.RegisterEffects(
            new ResourceGain("wood", 5),
            new ResourceCost("gold", 5),
            new TimeMult(5)
        );

        card.textComponent.text = "Wood\n(imported)";
    }


    class TimeMult : CardEffect {
        public TimeMult(float mult){
            time_mult = mult;
        }
    }

    class ResourceGain : CardEffect {
        string key;
        int amount;

        public ResourceGain(string key, int amount){
            this.key = key;
            this.amount = amount;
        }

        public override void Proc() => card.GainResources(key, amount);
    }

    class ResourceCost : CardEffect {
        string key;
        int amount;

        public ResourceCost(string key, int amount){
            this.key = key;
            this.amount = amount;
        }

        public override bool Condition() => GameManager.GetResourceAmount(key) >= amount;
        public override void Proc() => card.SpendResources(key, amount);
    }
}
