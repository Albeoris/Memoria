using UnityEngine;

public partial class EventEngine
{
    private void turnOffTriManually(int sid)
    {
        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1900)
        {
            if (sid != 4)
                return;
            this.fieldmap.walkMesh.BGI_triSetActive(56U, 0U);
        }
        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 900)
        {
            int varManually1 = this.eBin.getVarManually(EBin.SC_COUNTER_SVR);
            int varManually2 = this.eBin.getVarManually(EBin.MAP_INDEX_SVR);
            switch (sid)
            {
                case 8:
                    if (varManually1 < 4450)
                        break;
                    this.fieldmap.walkMesh.BGI_triSetActive(56U, 0U);
                    break;
                case 17:
                    if (varManually1 >= 4450 || varManually2 == 1 || varManually2 == 5)
                        break;
                    this.fieldmap.walkMesh.BGI_triSetActive(62U, 0U);
                    break;
            }
        }
        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1455)
        {
            if (sid != 5)
                return;
            this.fieldmap.walkMesh.BGI_triSetActive(16U, 0U);
        }
        else
        {
            if ((int)FF9StateSystem.Common.FF9.fldMapNo != 2803 || sid != 20)
                return;
            uint isActive = this.eBin.getVarManually(761060) != 1 ? 0U : 1U;
            this.fieldmap.walkMesh.BGI_triSetActive(105U, isActive);
            this.fieldmap.walkMesh.BGI_triSetActive(106U, isActive);
        }
    }
}