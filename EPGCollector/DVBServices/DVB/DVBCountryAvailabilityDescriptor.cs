﻿////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2016 nzsjb                                           //
//                                                                              //
//  This Program is free software; you can redistribute it and/or modify        //
//  it under the terms of the GNU General Public License as published by        //
//  the Free Software Foundation; either version 2, or (at your option)         //
//  any later version.                                                          //
//                                                                              //
//  This Program is distributed in the hope that it will be useful,             //
//  but WITHOUT ANY WARRANTY; without even the implied warranty of              //
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                //
//  GNU General Public License for more details.                                //
//                                                                              //
//  You should have received a copy of the GNU General Public License           //
//  along with GNU Make; see the file COPYING.  If not, write to                //
//  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.       //
//  http://www.gnu.org/copyleft/gpl.html                                        //
//                                                                              //  
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.ObjectModel;
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// DVB Country Availability descriptor class.
    /// </summary>
    internal class DVBCountryAvailabilityDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the availability flag.
        /// </summary>
        public bool AvailabilityFlag { get { return (availabilityFlag); } }

        /// <summary>
        /// Get the list of country codes.
        /// </summary>
        public Collection<string> CountryCodes { get { return (countryCodes); } }

        /// <summary>
        /// Get the index of the next byte in the section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("CountryAvailabilityDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private bool availabilityFlag;
        private Collection<string> countryCodes;
        
        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBCountryAvailabilityDescriptor class.
        /// </summary>
        internal DVBCountryAvailabilityDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                if (Length != 0)
                {
                    availabilityFlag = (byteData[lastIndex] & 0x80) != 0;
                    lastIndex++;

                    int countryCount = (Length - 1) / 3;

                    if (countryCount != 0)
                    {
                        countryCodes = new Collection<string>();

                        while (countryCodes.Count != countryCount)
                        {
                            countryCodes.Add(Utils.GetString(byteData, lastIndex, 3));
                            lastIndex += 3;
                        }
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Country Availability Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            StringBuilder countryString = new StringBuilder();

            if (countryCodes != null && countryCodes.Count != 0)
            {
                foreach (string countryCode in countryCodes)
                {
                    if (countryString.Length != 0)
                        countryString.Append(",");

                    countryString.Append(countryCode);
                }
            }
            else
                countryString.Append("not present");

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB COUNTRY AVAILABILITY DESCRIPTOR: Flag: " + availabilityFlag +
                " Country codes: " + countryString);            
        }
    }
}
