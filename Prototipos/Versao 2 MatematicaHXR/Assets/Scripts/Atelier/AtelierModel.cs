using System;
namespace MatematicaHXR.Atelier
{
    // Membership changes only on deliberate drops; holding never creates or destroys a unit.
    public sealed class AtelierModel
    {
        public const int Total = 15;
        public enum Stage { Explore, Split, Gather, Result }
        public Stage Phase { get; private set; }
        public readonly int[] Group = new int[Total];
        public readonly bool[] Collected = new bool[Total];
        public int Left { get { int n=0;foreach(int g in Group)if(g==0)n++;return n; } }
        public int Right => Total-Left;
        public int Joined { get { int n=0;foreach(bool c in Collected)if(c)n++;return n; } }
        public bool Practiced { get; private set; }
        public bool TransferRequired { get; private set; }
        public int BeforeLeft { get; private set; } = -1;
        public int TargetLeft { get; private set; } = -1;
        public void Practice() => Practiced=true;
        public void Begin()
        { if(!Practiced)return;Array.Clear(Group,0,Total);Array.Clear(Collected,0,Total);Phase=Stage.Split;BeforeLeft=-1;TransferRequired=false; }
        public bool Place(int id,int zone)
        {
            if(id<0||id>=Total||zone<0||zone>2)return false;
            if(Phase==Stage.Split && zone<2)
            {
                if(TransferRequired && (Group[id]!=0 || zone!=1 || Left!=TargetLeft+1))return false;
                if(!TransferRequired && BeforeLeft>=0 && Group[id]!=zone)return false;
                Group[id]=zone;
                if(TransferRequired && Left==TargetLeft)TransferRequired=false;
                return true;
            }
            if(Phase==Stage.Gather)
            {
                if(zone==2)Collected[id]=true;
                else if(zone==Group[id])Collected[id]=false;
                else return false;
                if(Joined==Total)Phase=Stage.Result;
                return true;
            }
            return false;
        }
        public bool BeginGather()
        { if(Phase!=Stage.Split||TransferRequired)return false;Array.Clear(Collected,0,Total);Phase=Stage.Gather;return true; }
        public void Separate()
        {Array.Clear(Collected,0,Total);Phase=Stage.Split;BeforeLeft=-1;TransferRequired=false;}
        public void NextPartition()
        {
            if(Phase!=Stage.Result)return;
            BeforeLeft=Left;
            if(Left==0){Array.Clear(Group,0,Total);BeforeLeft=-1;TargetLeft=-1;TransferRequired=false;}
            else{TargetLeft=Left-1;TransferRequired=true;}
            Array.Clear(Collected,0,Total);Phase=Stage.Split;
        }
    }
}
