<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Triptitude.Web.Azure1" generation="1" functional="0" release="0" Id="04d61806-cc96-4aee-b78b-0e6a25bf2041" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="Triptitude.Web.Azure1Group" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Triptitude.Web:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/LB:Triptitude.Web:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Triptitude.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/MapTriptitude.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="Triptitude.WebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/MapTriptitude.WebInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Triptitude.Web:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/Triptitude.Web/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapTriptitude.Web:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/Triptitude.Web/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapTriptitude.WebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/Triptitude.WebInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Triptitude.Web" generation="1" functional="0" release="0" software="C:\Source\triptitudenew\Triptitude.Web.Azure1\csx\Release\roles\Triptitude.Web" entryPoint="base\x86\WaHostBootstrapper.exe" parameters="base\x86\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Triptitude.Web&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Triptitude.Web&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/Triptitude.WebInstances" />
            <sCSPolicyFaultDomainMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/Triptitude.WebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="Triptitude.WebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="Triptitude.WebInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="0fdf94cf-c26e-40ee-8dd1-cf3659e13e6e" ref="Microsoft.RedDog.Contract\ServiceContract\Triptitude.Web.Azure1Contract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="42c18c8e-b410-45ee-a36a-4c2803241e06" ref="Microsoft.RedDog.Contract\Interface\Triptitude.Web:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Triptitude.Web.Azure1/Triptitude.Web.Azure1Group/Triptitude.Web:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>