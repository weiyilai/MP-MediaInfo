#region Copyright (C) 2017-2026 Yaroslav Tatarenko

// Copyright (C) 2017-2026 Yaroslav Tatarenko
// This product uses MediaInfo library, Copyright (c) 2002-2026 MediaArea.net SARL. 
// https://mediaarea.net

#endregion

using System;
using System.Collections.Generic;
using MediaInfo.Model;

namespace MediaInfo.Builder
{
  /// <summary>
  /// Builds video tag metadata by extracting and parsing video stream information from the provided media source.
  /// </summary>
  /// <remarks>This class is intended for internal use when constructing video tag data. It processes all
  /// available video tag fields defined in the native video enumeration and adds them to the resulting tag
  /// collection.
  /// </remarks>
  /// <param name="mediaInfo">The media information source from which video stream data is retrieved. Cannot be null.</param>
  /// <param name="streamPosition">The zero-based index of the video stream to process within the media source. Must be non-negative.</param>
  internal class VideoTagBuilder(MediaInfo mediaInfo, int streamPosition) : GeneralTagBuilder<VideoTags>(mediaInfo, streamPosition)
  {
    #region Tag items

    private static readonly List<Tuple<NativeMethods.Video, ParseDelegate<object>>> GeneralTagItems;

    #endregion

        #endregion

        static VideoTagBuilder()
        {
#if NET8_0_OR_GREATER
            var values = Enum.GetValues<NativeMethods.Video>();
#else
            var values = typeof(NativeMethods.Video).GetEnumValues();
#endif
            GeneralTagItems = new List<Tuple<NativeMethods.Video, ParseDelegate<object>>>(values.Length);
            foreach (NativeMethods.Video item in values)
            {
                GeneralTagItems.Add(new Tuple<NativeMethods.Video, ParseDelegate<object>>(item, TagBuilderHelper.TryGetString));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoTagBuilder"/> class.
        /// </summary>
        /// <param name="mediaInfo">The media information.</param>
        /// <param name="streamPosition">The stream position.</param>
        public VideoTagBuilder(MediaInfo mediaInfo, int streamPosition)
          : base(mediaInfo, streamPosition)
        {
        }

        public override VideoTags Build()
        {
            var result = base.Build();
            foreach (var tagItem in GeneralTagItems)
            {
                var value = MediaInfo.Get(StreamKind.Video, StreamPosition, (int)tagItem.Item1);
                if (!string.IsNullOrEmpty(value) && tagItem.Item2(value, out var tagValue))
                {
                    result.VideoDataTags.Add(tagItem.Item1, tagValue);
                }
            }

            return result;
        }
    }
}