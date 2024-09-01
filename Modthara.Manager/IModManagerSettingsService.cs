
using System.IO.Abstractions;
using System.Text.Json;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

public interface IModManagerSettingsService : IPathProvider
{

    public Task LoadSettingsAsync();

    public Task SaveSettingsAsync();
}