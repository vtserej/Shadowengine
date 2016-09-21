﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing; 

namespace ShadowEngine.VisualControls
{
    public interface IControl
    {
        /// <summary>
        /// This method returns the accesible name of the control
        /// <summary>
        string AccesibleName();

        /// <summary>
        /// This method returns the Z - ORDER of the control
        /// <summary>
        int ZOrder();

        /// <summary>
        /// This method returns true if the control is no longer needed
        /// <summary>
        bool Discard();

        /// <summary>
        /// This method returns the rectangle area of the control
        /// <summary>
        //Rectangle GetRectangle();

        /// <summary>
        /// This method Sets the rectangle area of the control
        /// <summary>
        //void SetRectangle(Rectangle rect);

        /// <summary>
        /// This method performs class operations to initializate the control
        /// <summary>
        void Create();

        /// <summary>
        /// This method updates the control lettin him know the mouse position
        /// <summary>
        void Update(Cursor cursor);

        /// <summary>
        /// This method renders the control
        /// <summary>
        void Draw();
    }
}
