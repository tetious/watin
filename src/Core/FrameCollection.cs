#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System.Collections.Generic;
using WatiN.Core.Constraints;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="Frame" /> instances within a <see cref="Document"/>.
	/// </summary>
	public class FrameCollection : BaseComponentCollection<Frame, FrameCollection>
	{
        private readonly Constraint findBy;
        private readonly List<Frame> frames;

		public FrameCollection(DomContainer domContainer, INativeDocument htmlDocument)
		{
            findBy = Find.Any;

            frames = new List<Frame>();

            foreach (INativeDocument frameDocument in htmlDocument.Frames)
                frames.Add(new Frame(domContainer, frameDocument));
		}

        private FrameCollection(Constraint findBy, List<Frame> frames)
        {
            this.findBy = findBy;
            this.frames = frames;
        }

        /// <inheritdoc />
        protected override FrameCollection CreateFilteredCollection(Constraint findBy)
        {
            return new FrameCollection(this.findBy & findBy, frames);
        }

        /// <inheritdoc />
        protected override IEnumerable<Frame> GetComponents()
        {
            var context = new ConstraintContext();
            foreach (Frame frame in frames)
                if (frame.Matches(findBy, context))
                    yield return frame;
        }
    }
}