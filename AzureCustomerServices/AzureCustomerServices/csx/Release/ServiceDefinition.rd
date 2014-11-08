﻿<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="AzureCustomerServices" generation="1" functional="0" release="0" Id="0febb7f8-9b0f-42aa-a528-f7c6db037de4" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AzureCustomerServicesGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="CustomerServicesWebRole:Endpoint" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/LB:CustomerServicesWebRole:Endpoint" />
          </inToChannel>
        </inPort>
        <inPort name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/LB:CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCertificate|CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:dbConnString" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:dbConnString" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:MaxTryCount" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:MaxTryCount" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:PerfMonSampleRate" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:PerfMonSampleRate" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:PerfMonScheduledTransferPeriod" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:PerfMonScheduledTransferPeriod" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:ProcessQueueName" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:ProcessQueueName" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRole:RetrySleepInterval" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRole:RetrySleepInterval" />
          </maps>
        </aCS>
        <aCS name="CustomerServicesWebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/MapCustomerServicesWebRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:CustomerServicesWebRole:Endpoint">
          <toPorts>
            <inPortMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Endpoint" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapCustomerServicesWebRole:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/DataConnectionString" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:dbConnString" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/dbConnString" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:MaxTryCount" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/MaxTryCount" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:PerfMonSampleRate" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/PerfMonSampleRate" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:PerfMonScheduledTransferPeriod" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/PerfMonScheduledTransferPeriod" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:ProcessQueueName" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/ProcessQueueName" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRole:RetrySleepInterval" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/RetrySleepInterval" />
          </setting>
        </map>
        <map name="MapCustomerServicesWebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="CustomerServicesWebRole" generation="1" functional="0" release="0" software="C:\Users\apervaiz\Desktop\Hardware\AzureCustomerServices\AzureCustomerServices\csx\Release\roles\CustomerServicesWebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint" protocol="http" portRanges="80" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/SW:CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="dbConnString" defaultValue="" />
              <aCS name="MaxTryCount" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="PerfMonSampleRate" defaultValue="" />
              <aCS name="PerfMonScheduledTransferPeriod" defaultValue="" />
              <aCS name="ProcessQueueName" defaultValue="" />
              <aCS name="RetrySleepInterval" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;CustomerServicesWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;CustomerServicesWebRole&quot;&gt;&lt;e name=&quot;Endpoint&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="CustomerServicesWebRole.svclog" defaultAmount="[1000,1000,1000]" defaultSticky="true" kind="Directory" />
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="CustomerServicesWebRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="CustomerServicesWebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="CustomerServicesWebRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="0a9246fe-8878-4f77-a859-52f65fddd4ba" ref="Microsoft.RedDog.Contract\ServiceContract\AzureCustomerServicesContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="f2c25df3-ba8c-41c3-b518-68fdb1029425" ref="Microsoft.RedDog.Contract\Interface\CustomerServicesWebRole:Endpoint@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole:Endpoint" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="60de2ba0-5e70-45ac-81fe-1496dbb3a188" ref="Microsoft.RedDog.Contract\Interface\CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>