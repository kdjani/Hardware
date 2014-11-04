<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="AzureCustomerServices" generation="1" functional="0" release="0" Id="859e1c6a-9e4b-42b0-a855-799a2f507710" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AzureCustomerServicesGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="CustomerServicesWebRole:Endpoint" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/LB:CustomerServicesWebRole:Endpoint" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
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
      </channels>
      <maps>
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
          <role name="CustomerServicesWebRole" generation="1" functional="0" release="0" software="C:\Users\apervaiz\Documents\Visual Studio 2013\Projects\AzureCustomerServices 2\AzureCustomerServices\csx\Debug\roles\CustomerServicesWebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="dbConnString" defaultValue="" />
              <aCS name="MaxTryCount" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="PerfMonSampleRate" defaultValue="" />
              <aCS name="PerfMonScheduledTransferPeriod" defaultValue="" />
              <aCS name="ProcessQueueName" defaultValue="" />
              <aCS name="RetrySleepInterval" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;CustomerServicesWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;CustomerServicesWebRole&quot;&gt;&lt;e name=&quot;Endpoint&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="CustomerServicesWebRole.svclog" defaultAmount="[1000,1000,1000]" defaultSticky="true" kind="Directory" />
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
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
    <implementation Id="a3cf5133-5bbc-47c5-af18-3dd1310f1945" ref="Microsoft.RedDog.Contract\ServiceContract\AzureCustomerServicesContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="ab125a04-5e7a-43d6-93fe-91e7caafd06b" ref="Microsoft.RedDog.Contract\Interface\CustomerServicesWebRole:Endpoint@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzureCustomerServices/AzureCustomerServicesGroup/CustomerServicesWebRole:Endpoint" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>