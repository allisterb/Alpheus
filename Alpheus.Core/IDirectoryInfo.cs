using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpheus.IO
{
    public interface IDirectoryInfo : IFileSystemInfo
    {
        IDirectoryInfo Parent { get; }
        IDirectoryInfo Root { get; }
        IDirectoryInfo Create(string dir_path);
        IDirectoryInfo[] GetDirectories();
        IDirectoryInfo[] GetDirectories(string searchPattern);
        IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption);
        IFileInfo[] GetFiles();
        IFileInfo[] GetFiles(string searchPattern);
        IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption);

        /*
        void Create();
        //
        // Summary:
        //     Creates a directory using a System.Security.AccessControl.DirectorySecurity object.
        //
        // Parameters:
        //   directorySecurity:
        //     The access control to apply to the directory.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The directory specified by path is read-only or is not empty.
        //
        //   T:System.IO.IOException:
        //     The directory specified by path is read-only or is not empty.
        //
        //   T:System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   T:System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one or more
        //     invalid characters as defined by System.IO.Path.InvalidPathChars.
        //
        //   T:System.ArgumentNullException:
        //     path is null.
        //
        //   T:System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum length.
        //     For example, on Windows-based platforms, paths must be less than 248 characters,
        //     and file names must be less than 260 characters.
        //
        //   T:System.IO.DirectoryNotFoundException:
        //     The specified path is invalid, such as being on an unmapped drive.
        //
        //   T:System.NotSupportedException:
        //     Creating a directory with only the colon (:) character was attempted.
        public void Create(DirectorySecurity directorySecurity);
        public DirectoryInfo CreateSubdirectory(string path);
        //
        // Summary:
        //     Creates a subdirectory or subdirectories on the specified path with the specified
        //     security. The specified path can be relative to this instance of the System.IO.DirectoryInfo
        //     class.
        //
        // Parameters:
        //   path:
        //     The specified path. This cannot be a different disk volume or Universal Naming
        //     Convention (UNC) name.
        //
        //   directorySecurity:
        //     The security to apply.
        //
        // Returns:
        //     The last directory specified in path.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     path does not specify a valid file path or contains invalid DirectoryInfo characters.
        //
        //   T:System.ArgumentNullException:
        //     path is null.
        //
        //   T:System.IO.DirectoryNotFoundException:
        //     The specified path is invalid, such as being on an unmapped drive.
        //
        //   T:System.IO.IOException:
        //     The subdirectory cannot be created.-or- A file or directory already has the name
        //     specified by path.
        //
        //   T:System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum length.
        //     For example, on Windows-based platforms, paths must be less than 248 characters,
        //     and file names must be less than 260 characters. The specified path, file name,
        //     or both are too long.
        //
        //   T:System.Security.SecurityException:
        //     The caller does not have code access permission to create the directory.-or-The
        //     caller does not have code access permission to read the directory described by
        //     the returned System.IO.DirectoryInfo object. This can occur when the path parameter
        //     describes an existing directory.
        //
        //   T:System.NotSupportedException:
        //     path contains a colon character (:) that is not part of a drive label ("C:\").
        [SecuritySafeCritical]
        public DirectoryInfo CreateSubdirectory(string path, DirectorySecurity directorySecurity);
        [SecuritySafeCritical]
        public override void Delete();
        [SecuritySafeCritical]
        public void Delete(bool recursive);
        public IEnumerable<DirectoryInfo> EnumerateDirectories();
        public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern);
        public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption);
        public IEnumerable<FileInfo> EnumerateFiles();
        public IEnumerable<FileInfo> EnumerateFiles(string searchPattern);
        public IEnumerable<FileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption);
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos();
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern);
        public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption);
        //
        // Summary:
        //     Gets a System.Security.AccessControl.DirectorySecurity object that encapsulates
        //     the access control list (ACL) entries for the directory described by the current
        //     System.IO.DirectoryInfo object.
        //
        // Returns:
        //     A System.Security.AccessControl.DirectorySecurity object that encapsulates the
        //     access control rules for the directory.
        //
        // Exceptions:
        //   T:System.SystemException:
        //     The directory could not be found or modified.
        //
        //   T:System.UnauthorizedAccessException:
        //     The current process does not have access to open the directory.
        //
        //   T:System.UnauthorizedAccessException:
        //     The directory is read-only.-or- This operation is not supported on the current
        //     platform.-or- The caller does not have the required permission.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred while opening the directory.
        //
        //   T:System.PlatformNotSupportedException:
        //     The current operating system is not Microsoft Windows 2000 or later.
        public DirectorySecurity GetAccessControl();
        //
        // Summary:
        //     Gets a System.Security.AccessControl.DirectorySecurity object that encapsulates
        //     the specified type of access control list (ACL) entries for the directory described
        //     by the current System.IO.DirectoryInfo object.
        //
        // Parameters:
        //   includeSections:
        //     One of the System.Security.AccessControl.AccessControlSections values that specifies
        //     the type of access control list (ACL) information to receive.
        //
        // Returns:
        //     A System.Security.AccessControl.DirectorySecurity object that encapsulates the
        //     access control rules for the file described by the path parameter.ExceptionsException
        //     typeConditionSystem.SystemExceptionThe directory could not be found or modified.System.UnauthorizedAccessExceptionThe
        //     current process does not have access to open the directory.System.IO.IOExceptionAn
        //     I/O error occurred while opening the directory.System.PlatformNotSupportedExceptionThe
        //     current operating system is not Microsoft Windows 2000 or later.System.UnauthorizedAccessExceptionThe
        //     directory is read-only.-or- This operation is not supported on the current platform.-or-
        //     The caller does not have the required permission.
        public DirectorySecurity GetAccessControl(AccessControlSections includeSections);
        public FileSystemInfo[] GetFileSystemInfos();
        public FileSystemInfo[] GetFileSystemInfos(string searchPattern);
        public FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption);
        [SecuritySafeCritical]
        public void MoveTo(string destDirName);
        //
        // Summary:
        //     Applies access control list (ACL) entries described by a System.Security.AccessControl.DirectorySecurity
        //     object to the directory described by the current System.IO.DirectoryInfo object.
        //
        // Parameters:
        //   directorySecurity:
        //     An object that describes an ACL entry to apply to the directory described by
        //     the path parameter.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The directorySecurity parameter is null.
        //
        //   T:System.SystemException:
        //     The file could not be found or modified.
        //
        //   T:System.UnauthorizedAccessException:
        //     The current process does not have access to open the file.
        //
        //   T:System.PlatformNotSupportedException:
        //     The current operating system is not Microsoft Windows 2000 or later.
        public void SetAccessControl(DirectorySecurity directorySecurity);
        public override string ToString();
        */
    }
}
