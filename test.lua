DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
Duel = DuelLayout:GetComponent("DuelOperation")

function c71703785()
    if(Duel:DrawThisCard("71703785")) then
        Duel:SetChainableEffect(1, true)
    end
end

function c71703785cost1()
    print("马哈德发动需要代价")
end

function c71703785effect1()
    print("马哈德发动效果")
end