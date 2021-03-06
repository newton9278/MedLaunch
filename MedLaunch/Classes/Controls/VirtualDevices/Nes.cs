﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLaunch.Classes.Controls.VirtualDevices
{
    public class Nes
    {
        public static DeviceDefinition GamePad(int VirtualPort)
        {
            DeviceDefinition device = new DeviceDefinition();
            device.DeviceName = "NES GamePad";
            device.CommandStart = "nes.input.port" + VirtualPort;
            device.VirtualPort = VirtualPort;
            device.MapList = new List<Mapping>
            {
                new Mapping { Description = "UP ↑", MednafenCommand = device.CommandStart +".gamepad.up" },
                new Mapping { Description = "DOWN ↓", MednafenCommand = device.CommandStart +".gamepad.down" },
                new Mapping { Description = "LEFT ←", MednafenCommand = device.CommandStart +".gamepad.left" },
                new Mapping { Description = "RIGHT →", MednafenCommand = device.CommandStart +".gamepad.right" },
                new Mapping { Description = "START", MednafenCommand = device.CommandStart +".gamepad.start" },
                new Mapping { Description = "SELECT", MednafenCommand = device.CommandStart +".gamepad.select" },
                new Mapping { Description = "A", MednafenCommand = device.CommandStart +".gamepad.a" },
                new Mapping { Description = "B", MednafenCommand = device.CommandStart +".gamepad.b" },
                new Mapping { Description = "Rapid A", MednafenCommand = device.CommandStart +".gamepad.rapid_a" },
                new Mapping { Description = "Rapid B", MednafenCommand = device.CommandStart +".gamepad.rapid_b" },
            };
            DeviceDefinition.PopulateConfig(device);
            return device;
        }
    }

    
}
