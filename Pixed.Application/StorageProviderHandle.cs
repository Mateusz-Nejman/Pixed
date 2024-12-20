﻿using Avalonia.Platform.Storage;
using Pixed.Application.Platform;
using Pixed.Application.Utils;
namespace Pixed.Application;
internal class StorageProviderHandle : IStorageProviderHandle
{
    private IStorageProvider _storageProvider;
    public IStorageProvider StorageProvider => _storageProvider;
    public IPlatformFolder StorageFolder => PlatformUtils.platformFolder;
    public void Initialize(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
        StorageFolder.Initialize(storageProvider);
    }
}
