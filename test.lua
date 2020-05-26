DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
Duel = DuelLayout:GetComponent("DuelEvent")
duelData = Duel.duelData

function c71703785()
    if(c71703785condition1()) then
        Duel:Precheck()
        c71703785cost1()
        c71703785effect1()
        Duel:SetActivatableEffect(1, true)
    end
    if(c71703785condition2()) then
        
    end
end

function c71703785condition1()
    return Duel:DrawnCard("")
end

function c71703785cost1()
    Duel:ShowCard("")
end

function c71703785effect1()
    Duel:SpecialSummon("")
end

function c71703785condition2()
    if(duelData.duelPhase > GamePhase.damageStepStart) then
        print(111111)
    end
    --print(duelData)
    --print(Duel.duelData)
    --print(type(Duel:GetAntiMonster()))
    return true
end