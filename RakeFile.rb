require 'albacore'
require 'rexml/document'

def get_version
	ENV['BUILD_NUMBER'] || '1.0.0.38'
end

params = {
	:package_output		=> "packages",
	:build_output		=> "build",
	:version 			=> get_version(),
	:solution_file_name => "src/BuildingBlocks.sln",
	:nuget_apikey		=> ENV['apikey']
}

task :default  => "build:all"

namespace :ci do
	desc "buid and create packages"
	task :run => ["build:all", "package:create_packages"]

	desc "build and create packages and push to nuget server"	
	task :push, :nuget_apikey do |t, args|
		Rake::Task["package:push"].execute(args[:nuget_apikey])
	end
	task :push => ["ci:run"]
end

namespace :source do
	desc 'Update assembly info with latest version number'
	assemblyinfo :update_version do |asm|
	    asm.output_file = 'src/CommonAssemblyInfo.cs'
	    asm.version = params[:version]
	    asm.company_name = 'Building-Blocks'
	    asm.copyright = 'Copyright 2012 Innokentiy Filipenko. All rights reserved.'
	    asm.namespaces = ['System.Security']
	    puts "The build number is #{asm.version}"
  end
end

namespace :build do
	desc "build solution"
	task :all => ['source:update_version', :build_release, :clean_output, :copy_to_output]

	msbuild :build_release do |msb|
		msb.properties :configuration => :Release
		msb.targets = [ :Clean, :Build ]
		msb.solution = params[:solution_file_name]
	end

	desc "Copying output to build directory"
	task :copy_to_output do
		puts "Copying output to directory '#{params[:build_output]}'"
		Dir.mkdir params[:build_output] unless File.exist? params[:build_output]
		Dir.glob 'src/BuildingBlocks.*/bin/Release/BuildingBlocks.*.{dll,pdb,xml,nuspec}' do |path|
			copy path, params[:build_output] if File.file? path
		end
	end

	task :clean_output do
		FileUtils.rm_rf params[:build_output]
    	puts "Directory '#{params[:build_output]}' removed"
	end

	nunit :run_tests do |nunit|
    	nunit.path_to_command = "tools/nunit/nunit-console.exe"
    	nunit.assemblies "build_output/Building-Blocks.dll"
	end
end

namespace :package do
	desc "creating Nuget packages for every projects in solution"
	task :all => [:create_packages]

	task :create_packages do
		renew_packages_dir(params)
		nuspec_list = FileList.new("#{params[:build_output]}/*.nuspec").to_a
		nuspec_list.each do |f|
			if not is_file_of_tests_project(f)
				if is_assmblies_package(f)
					packageName = f.pathmap("%n")
					patch_nuspec_file_version(f, params)
					packageDir = copy_nuspec_to_packages_dir(f, packageName, params)
					copy_binaries_to_libs_dir(packageDir, packageName, params)
					newNuspecFile = "#{packageDir}/" + f.pathmap("%f")
					nuget_pack(newNuspecFile, packageDir, params)
				else
					nuget_pack(f, nil, params)
				end
			end
		end
	end

	task  :push, :nuget_apikey do |t, nuget_apikey|
		FileList.new("#{params[:package_output]}/*.nupkg").each do |f|
    		nuget_push(f, nuget_apikey, params)
		end
	end

	def copy_nuspec_to_packages_dir(nuspecfile, packageName, params)
		packageDir = "#{params[:package_output]}/#{packageName}"
		Dir.chdir(params[:package_output])
		Dir.mkdir packageName unless Dir.exist? packageName
		Dir.chdir("..")
		copy nuspecfile, packageDir if File.file? nuspecfile
		packageDir
	end

	def copy_binaries_to_libs_dir(packageDir, packageName, params)
		dir = Dir.pwd
		Dir.chdir(packageDir)
		Dir.mkdir "lib" unless Dir.exist? "lib"
		Dir.chdir("lib")
		Dir.mkdir "net40" unless Dir.exist? "net40"
		Dir.chdir(dir)

		libsDir = "#{packageDir}/lib/net40/"

		FileList.new("#{params[:build_output]}/#{packageName}.dll").each do |path|
			copy path, libsDir if File.file? path
		end		

		libsDir
	end

	def renew_packages_dir(params)
		if (Dir.exist? params[:package_output])
			FileUtils.rm_rf params[:package_output]
		end
		Dir.mkdir params[:package_output]
		puts "Directory '#{params[:package_output]}' clear"
	end

	def is_assmblies_package(nuspecfile)
		File.exist? nuspecfile.ext('dll')
	end
	
	def is_file_of_tests_project(file)
		file.pathmap("%n").end_with?(".Tests") or file.pathmap("%n").end_with?(".tests") or file.pathmap("%n").end_with?(".test") or file.pathmap("%n").end_with?(".tests")
	end	

	def nuget_pack(nuspec_file, content_dir, params)
		puts ""
		puts "Creating package for #{nuspec_file}"

    	cmd = Exec.new
    	output = params[:package_output]
    	cmd.command = 'src/.nuget/NuGet.exe'
    	cmd.parameters = "pack #{nuspec_file} -basepath #{content_dir} -outputdirectory #{output}"
    	cmd.execute

    	puts "Package for #{nuspec_file} created"
		puts ""
  	end	

  	def nuget_push(nupkg_file, api_key, params)
		puts ""
		puts "Pushing package #{nupkg_file}"

		dirBefore = Dir.pwd
		Dir.chdir(params[:package_output])

		begin
			nugetpath = "../src/.nuget/NuGet.exe";

			cmdSetApiKey = Exec.new
    		cmdSetApiKey.command = nugetpath
    		cmdSetApiKey.parameters = "SetApiKey #{api_key}"

    		cmdPush = Exec.new
    		nupkg_file = nupkg_file.pathmap("%f")
    		cmdPush.command = nugetpath
    		cmdPush.parameters = "Push #{nupkg_file}"

	    	cmdSetApiKey.execute
    		cmdPush.execute
		ensure
			Dir.chdir(dirBefore)			
		end    	
  	end	

  	def patch_nuspec_file_version(nuspecfile, params)
  		contents = File.new(nuspecfile).read
  		doc = REXML::Document.new(contents)
  		verElm = doc.root.elements["metadata/version"]
  		verElm.text = params[:version]
  		doc.root.elements.each("metadata/dependencies/dependency") do |elm|
  			package = elm.attributes["id"]
            if package.start_with? 'BuildingBlocks'
            	elm.attributes["version"] = params[:version]
            end
  		end

		formatter = REXML::Formatters::Default.new
		File.open(nuspecfile, 'w') do |result|
			formatter.write(doc, result)
		end
  	end

	task :create_nuspec do
		assemblyList = FileList.new("#{params[:build_output]}/*.dll").to_a
		assemblyList.each do |f|
			nuspecfile = f.ext('nuspec')
			Rake::Task["create_spec"].execute(nuspecfile)
		end
	end
end