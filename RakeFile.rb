require 'albacore'

def get_version
	ENV['BUILD_NUMBER'] || '1.0.0.0'
end

params = {
	:package_output		=> "packages",
	:build_output		=> "build",
	:version 			=> get_version(),
	:solution_file_name => "src/BuildingBlocks.sln"	
}

task :default  => "build:all"

namespace :ci do
	desc "create TeamCity artifacts"
	task :run => [
		"build:all",
		"package:all"
	]
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
	task :all => [:clean_output, :create_packages]

	task :create_packages do
		nuspec_list = FileList.new("#{params[:build_output]}/*.nuspec").to_a
		nuspec_list.each do |f|			
			Rake::Task["package:create_package"].execute(f)
		end
	end

	nugetpack :create_package, :nuspec_file do |nuget, nuspec_file|
		puts ""
		puts "Creating package for #{nuspec_file}"
		Dir.mkdir params[:package_output] unless File.exist? params[:package_output]

		nuget.command     = "src/.nuget/nuget.exe"
		nuget.nuspec      = nuspec_file
		nuget.output      = params[:package_output]

		puts "Package for #{nuspec_file} created"
	end

	task :clean_output do
		FileUtils.rm_rf params[:package_output]
    	puts "Directory '#{params[:package_output]}' removed"
	end

	task :create_nuspec do
		assemblyList = FileList.new("#{params[:build_output]}/*.dll").to_a
		assemblyList.each do |f|
			nuspecfile = f.ext('nuspec')
			Rake::Task["create_spec"].execute(nuspecfile)
		end
	end
end