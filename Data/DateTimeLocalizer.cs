using System;
using Microsoft.JSInterop;

// See: https://www.meziantou.net/convert-datetime-to-user-s-time-zone-with-server-side-blazor.htm

namespace AnyDex.Data {
    /// <summary>
    /// Converts any <see cref="DateTime"/> value to the Client's Timezone equivalent.
    /// </summary>
    public sealed class DateTimeLocalizer {
        private readonly IJSRuntime _jsRuntime;

        private TimeSpan? _userOffset;

        public DateTimeLocalizer(IJSRuntime jsRuntime) {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Asynchronously receive the <see cref="DateTime.Now"/> value adapted to the Client's Timezone.
        /// </summary>
        public async ValueTask<DateTimeOffset> NowAsync()
            => await GetLocalDateTimeAsync(DateTimeOffset.Now);

        /// <summary>
        /// Asynchronously receive the value of the <paramref name="dateTime"/>, adapted to the Client's Timezone.
        /// </summary>
        /// <param name="dateTime">
        /// The <see cref="DateTime"/> value to convert.
        /// </param>
        public async ValueTask<DateTimeOffset> GetLocalDateTimeAsync(DateTimeOffset dateTime) {
            if(_userOffset == null) {
                int offsetInMinutes = await _jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
                _userOffset = TimeSpan.FromMinutes(-offsetInMinutes);
            }
            return dateTime.ToOffset(_userOffset.Value);
        }
    }
}
