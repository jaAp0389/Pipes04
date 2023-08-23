/*****************************************************************************
* Project: Pipesgame
* File   : AudioManager.cs
* Date   : 04.06.2021
* Author : Jan Apsel (JA)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*   04.06.2021	JA	Finished
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
    /// <summary>
    /// is placed on map and can be pressed by water
    /// </summary>
    public class Switch
    {
        private int switchListPos { get; set; }
        public Switch(int _switchListPos)
        {
            switchListPos = _switchListPos;
        }
        public void PressSwitch()
        {
            Database.s_switchList[switchListPos] -= 1;
        }
    }
}
