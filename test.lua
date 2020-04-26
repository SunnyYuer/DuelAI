DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
Duel = DuelLayout:GetComponent("DuelEvent")

function c71703785()
    if(Duel:InActivePhase()) then
    end
    if(Duel:DrawnCard("")) then
        Duel:Precheck()
        c71703785cost1()
        c71703785effect1()
        Duel:SetActivatableEffect(1, true)
    end
end

function c71703785cost1()
    Duel:ShowCard("")
end

function c71703785effect1()
    Duel:SpecialSummon("")
end