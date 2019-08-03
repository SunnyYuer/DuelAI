DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
DuelOperation = DuelLayout:GetComponent("DuelOperation")

function Card71703785()
    if(DuelOperation.DrawThisCard("71703785")) then
        print(1234)
    else
        print(4567)
    end
    DuelOperation:Log()
end