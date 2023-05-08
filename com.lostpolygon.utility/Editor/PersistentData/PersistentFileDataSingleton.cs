using System.IO;
using UnityEditor;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    public abstract class PersistentFileDataSingleton<TData> where TData : class {
        protected static PersistentFileDataSingleton<TData> _instance;

        private TData _data;
        private readonly FileSystemWatcher _fileSystemWatcher;
        private bool _shouldIgnoreNextChange;

        protected string FilePath { get; }

        public TData InstanceData {
            get {
                if (_data == null) {
                    Load(false);
                }

                return _data;
            }
            protected set => _data = value;
        }

        protected PersistentFileDataSingleton(string filePath, bool monitorFileChanges) {
#if LP_TRACE
            Debug.Log(
                $"[PersistentFileDataSingleton<{typeof(TData).Name}>] " +
                $"created (filePath = {filePath}, monitorFileChanges = {monitorFileChanges})"
            );
#endif
            FilePath = filePath;

            if (_instance != null) {
                Debug.LogError(
                    $"{nameof(PersistentFileDataSingleton<TData>)} already exists. " +
                    "Did you query the singleton in the constructor?"
                );
                return;
            }

            _instance = this;

            if (monitorFileChanges) {
                _fileSystemWatcher = new FileSystemWatcher(
                    Path.GetDirectoryName(FilePath),
                    Path.GetFileName(FilePath)
                );

                _fileSystemWatcher.NotifyFilter =
                    NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Security
                    | NotifyFilters.Size;

                void OnFileChanged() {
#if LP_TRACE
                    Debug.Log(
                        $"[PersistentFileDataSingleton<{typeof(TData).Name}>] " +
                        $"OnFileChanged(ignored = {_shouldIgnoreNextChange})"
                    );
#endif
                    if (_shouldIgnoreNextChange) {
                        _shouldIgnoreNextChange = false;
                        return;
                    }

                    CallLoadOnMainThread();
                }

                _fileSystemWatcher.Changed += (_, _) => OnFileChanged();
                _fileSystemWatcher.Created += (_, _) => OnFileChanged();
                _fileSystemWatcher.Deleted += (_, _) => OnFileChanged();
                _fileSystemWatcher.Renamed += (_, _) => OnFileChanged();
                _fileSystemWatcher.Error += (_, _) => OnFileChanged();

                _fileSystemWatcher.IncludeSubdirectories = false;
                _fileSystemWatcher.EnableRaisingEvents = true;

                Application.unloading += () => {
                    _fileSystemWatcher?.Dispose();
                };
            }
        }

        protected virtual void CallLoadOnMainThread() {
            EditorApplication.delayCall += () => {
                Load(true);
            };
        }

        public void Save() {
#if LP_TRACE
            Debug.Log(
                $"[PersistentFileDataSingleton<{typeof(TData).Name}>] " +
                $"Save({_data})!"
            );
#endif
            if (InstanceData == null) {
                Debug.LogError(
                    $"[PersistentFileDataSingleton<{typeof(TData).Name}>] " +
                    "Cannot save data: no instance!"
                );
                return;
            }

            string directoryName = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }

            SaveInstanceData();
            _shouldIgnoreNextChange = true;
        }

        public virtual void Delete() {
#if LP_TRACE
            Debug.Log(
                $"[PersistentFileDataSingleton<{typeof(TData).Name}>] " +
                $"Delete"
            );
#endif

            File.Delete(FilePath);
        }

        public abstract void Load(bool changeDetected);

        protected abstract void SaveInstanceData();

        ~PersistentFileDataSingleton() {
#if LP_TRACE
            Debug.Log(
                $"[PersistentFileDataSingleton<{typeof(TData).Name}>] " +
                $"destruction (filePath = {FilePath})"
            );
#endif
        }
    }
}
