DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
Duel = DuelLayout:GetComponent("DuelOperation")

function c71703785()
    if(Duel:DrawnCard("")) then
        Duel:SetChainableEffect(1, 2, true)
    end
end

function c71703785cost1()
    Duel:ShowCard("")
end

function c71703785effect1()
    Duel:SpecialSummon("")
end