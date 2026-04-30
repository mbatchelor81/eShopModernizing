FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
ARG source
WORKDIR /inetpub/wwwroot
EXPOSE 80
COPY ${source:-obj/Docker/publish} .

# Security: create non-root user and grant read access to web content (CWE-250)
RUN powershell -Command "New-LocalUser -Name 'appuser' -NoPassword -AccountNeverExpires; \
    Add-LocalGroupMember -Group 'IIS_IUSRS' -Member 'appuser'"
RUN icacls "C:\inetpub\wwwroot" /grant "appuser:(OI)(CI)R" /T
USER appuser
