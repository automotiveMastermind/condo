require 'yaml'

module Bridge
  class Generator < Jekyll::Generator
    def generate(site)
      path = File.join(site.source, "_data/bridge.json")
      site.data["bridge"] = YAML.load_file(path)
    end
  end
end