# Uninstalling

## Directories to remove

- `/opt/ChristmasPi/`
- `/opt/dotnet/`

## Files to remove

- `/etc/systemd/system/ChristmasPi.service`
- `/etc/systemd/system/Scheduler.service`

Also see `helpers/uninstall-services.sh`

## Files to edit

- `/home/pi/.bashrc`
    - Remove `export PATH="/opt/dotnet/:$PATH"`
    - Remove `export DOTNET_ROOT="/opt/dotnet/"`