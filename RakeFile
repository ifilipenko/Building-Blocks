require 'fileutils'
require 'albacore'
require './tools/albacore/nuspec_patch'

def get_version
  ENV['BUILD_NUMBER'] || '0.1.0.0'
end

task :default => 'build:all'

namespace :ci do
  task :run_ci_build => [
    'build:all',    
    'package:all',
  ]
end

namespace :source do
  desc 'Update assembly info with latest version number'
  assemblyinfo :update_version do |asm|
    asm.output_file = 'src/CommonAssemblyInfo.cs'
    
    asm.version = get_version
    asm.company_name = 'Building-Blocks'
    asm.copyright = 'Copyright 2012 Innokentiy Filipenko. All rights reserved.'
    asm.namespaces = ['System.Security']
    #asm.custom_attributes :AllowPartiallyTrustedCallers => nil
    
    puts "The build number is #{asm.version}"
  end
  
  desc 'Compile the source'
  msbuild :compile do |msb|
    msb.properties = { :configuration => :Release }
    msb.targets [:Clean, :Build]
    msb.solution = 'src/BuildingBlocks.sln'
  end
end

namespace :build do
  desc 'Run full build including tests'
  task :all => ['source:update_version', 'source:compile'] do
    puts 'Copying output to build directory'
	  FileUtils.rm_rf 'build'
      Dir.mkdir 'build' unless File.exist? 'build'
      Dir.glob 'src/BuildingBlocks.*/bin/Release/BuildingBlocks.*.{dll,pdb,xml}' do |path|
		copy path, 'build' if File.file? path
      end
	  
	  FileUtils.rm_rf 'build-all'
	  Dir.mkdir 'build-all'
	  Dir.glob 'src/BuildingBlocks.*/bin/Release/*.{dll,pdb,xml}' do |path|		
		copy path, 'build-all' if File.file? path
      end
    
    puts 'Build complete'
  end
end

namespace :package do
  task :prepare_dist_directory do
    FileUtils.rm_rf 'dist'
    Dir.mkdir 'dist'
  end
  
  desc "create the BuildingBlock nuspec file"
  nuspec :create_spec do |nuspec|
     version = "#{get_version}"

     nuspec.id = "Building-Blocks"
     nuspec.version = version
     nuspec.authors = "Innokentiy Filipenko"
     nuspec.owners = "Innokentiy Filipenko"
     nuspec.description = "Building Blocks."
     nuspec.title = "Building Blocks"
     nuspec.language = "en-US"
     nuspec.projectUrl = "https://github.com/ifilipenko/Building-Blocks"
     nuspec.working_directory = "build"
     nuspec.output_file = "Building-Blocks.nuspec"
  end
  
  def nuget_pack(base_folder, nuspec_path)
    cmd = Exec.new
    output = 'nuget/'
    cmd.command = 'src/.nuget/NuGet.exe'
    cmd.parameters = "pack #{nuspec_path} -basepath #{base_folder} -outputdirectory #{output}"
    cmd.execute
  end
  
  task :make_nuget_packages do
	nuget_pack('dist', 'build/Building-Blocks.nuspec')
  end
  
  desc 'Copy builded nuget package files'  
  task :copy do 
    Dir.glob 'src/BuildingBlocks.*/bin/Release/*.nupkg' do |path|		
		copy path, 'dist' if File.file? path
    end
  end
  
  def get_exclusions
    exclusions = []
    %w{build dist results output}.each {|x| exclusions << "#{x}" << "#{x}/**/**" }
    %w{bin obj}.each {|x| exclusions << "**/#{x}" << "**/#{x}/**/**" }
    [/_ReSharper/, /.user/, /.suo/, /.resharper/, /.cache/].each {|x| exclusions << x }
    exclusions
  end 
    
  desc 'Create zip of binaries'
  zip :binaries do |zip|
    file_prefix = ENV['BinaryDistFilename'] || 'building-blocks-binary'
    zip.directories_to_zip = ['build']
    zip.output_file = "#{file_prefix}-#{get_version}.zip"
    zip.output_path = 'dist'
  end
  
  desc 'Create zip of binaries with dependencies'
  zip :binariesfull do |zip|
    file_prefix = ENV['BinaryDistFilename'] || 'building-blocks-binary'
    zip.directories_to_zip = ['build-all']
    zip.output_file = "#{file_prefix}-all-#{get_version}.zip"
    zip.output_path = 'dist'
  end
  
  desc 'Package everything (src, bin, nuget)'
  task :all => [:prepare_dist_directory, :binaries, :create_spec, :binariesfull, :copy, :make_nuget_packages]
end

task :sln do
  Thread.new do
    system "devenv src/BuildingBlocks.sln"
  end
end
