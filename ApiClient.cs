using System.Net.Http.Headers;
using System.Text.Json;
using RaceDayMVC.Models;

namespace RaceDayMVC.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _ctx;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public ApiClient(HttpClient http, IHttpContextAccessor ctx)
        {
            _http = http;
            _ctx = ctx;
        }

        private void AttachToken()
        {
            var token = _ctx.HttpContext?.Session.GetString("Token");
            _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
                ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<LoginResult?> LoginAsync(LoginVm vm)
        {
            var payload = new
            {
                Email = vm.Email.Trim().ToLowerInvariant(),
                Password = vm.Password
            };

            var res = await _http.PostAsJsonAsync("Auth/login", payload);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<LoginResult>(_json);
        }

        public async Task<(bool ok, string? error)> RegisterAsync(RegisterVm vm)
        {
            var res = await _http.PostAsJsonAsync("Auth/register", vm);
            if (res.IsSuccessStatusCode) return (true, null);
            return (false, await res.Content.ReadAsStringAsync());
        }

        public async Task<List<EventListVm>> GetEventsAsync()
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<EventListVm>>("Events", _json) ?? new();
        }

        public async Task<List<EventListVm>> GetMyEventsAsync(int organiserId)
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<EventListVm>>($"Events/organiser/{organiserId}", _json) ?? new();
        }

        public async Task<(bool ok, string? error)> CreateEventAsync(CreateEventVm vm, int organiserId)
        {
            AttachToken();

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(vm.Name), "Name");
            if (vm.Description != null) form.Add(new StringContent(vm.Description), "Description");
            form.Add(new StringContent(vm.Location), "Location");
            form.Add(new StringContent(vm.EventDate.ToString("o")), "EventDate");
            form.Add(new StringContent(organiserId.ToString()), "OrganiserId");
            form.Add(new StringContent(vm.Distance.ToString()), "Distance");
            form.Add(new StringContent(vm.EventType), "EventType");

            if (vm.Banner != null && vm.Banner.Length > 0)
            {
                var fileContent = new StreamContent(vm.Banner.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(vm.Banner.ContentType);
                form.Add(fileContent, "banner", vm.Banner.FileName);
            }

            var res = await _http.PostAsync("Events", form);
            if (res.IsSuccessStatusCode) return (true, null);

            return (false, await res.Content.ReadAsStringAsync());
        }
        // Add these methods inside the ApiClient class:

        public async Task<EventDetailVm?> GetEventAsync(int id)
        {
            AttachToken();
            return await _http.GetFromJsonAsync<EventDetailVm>($"Events/{id}", _json);
        }

        public async Task<(bool ok, string? error)> EnrolAsync(int eventId, int participantId)
        {
            AttachToken();
            var payload = new { ParticipantId = participantId, RaceEventId = eventId };
            var res = await _http.PostAsJsonAsync("Registrations", payload);

            if (res.IsSuccessStatusCode) return (true, null);
            return (false, await res.Content.ReadAsStringAsync());
        }

        public async Task<List<RegistrationVm>> GetMyRegistrationsAsync(int participantId)
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<RegistrationVm>>($"Registrations/participant/{participantId}", _json) ?? new();
        }

        public async Task<List<RegistrationVm>> GetEventRegistrationsAsync(int eventId)
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<RegistrationVm>>($"Registrations/event/{eventId}", _json) ?? new();
        }
        public async Task<ParticipantProfileVm?> GetProfileAsync(int participantId)
        {
            AttachToken();
            return await _http.GetFromJsonAsync<ParticipantProfileVm>($"Participants/{participantId}", _json);
        }

        public async Task<bool> UpdateProfileAsync(ParticipantProfileVm vm)
        {
            AttachToken();
            var form = new MultipartFormDataContent();
            if (vm.EmergencyContact != null) form.Add(new StringContent(vm.EmergencyContact), "emergencyContact");

            if (vm.PictureFile != null && vm.PictureFile.Length > 0)
            {
                var fileContent = new StreamContent(vm.PictureFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(vm.PictureFile.ContentType);
                form.Add(fileContent, "profilePicture", vm.PictureFile.FileName);
            }

            var res = await _http.PutAsync($"Participants/{vm.ParticipantId}", form);
            return res.IsSuccessStatusCode;
        }
    }
}