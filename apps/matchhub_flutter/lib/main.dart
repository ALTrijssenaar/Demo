import 'package:flutter/material.dart';

import 'score_service.dart';

void main() {
  const apiBaseUrl = String.fromEnvironment('MATCHHUB_API_URL', defaultValue: 'http://localhost:5270');
  const apiToken = String.fromEnvironment('MATCHHUB_API_TOKEN', defaultValue: '');
  runApp(
    MatchHubApp(
      scoreService: HttpScoreService(baseUrl: apiBaseUrl, bearerToken: apiToken),
    ),
  );
}

class MatchHubApp extends StatelessWidget {
  const MatchHubApp({required this.scoreService, super.key});

  final ScoreService scoreService;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'MatchHub Score Entry',
      theme: ThemeData(
        brightness: Brightness.light,
        useMaterial3: true,
        colorScheme: const ColorScheme.highContrastLight(
          primary: Color(0xFF0A4C8A),
          onPrimary: Colors.white,
          secondary: Color(0xFF00695C),
        ),
      ),
      home: ScoreEntryPage(scoreService: scoreService),
    );
  }
}

class ScoreEntryPage extends StatefulWidget {
  const ScoreEntryPage({required this.scoreService, super.key});

  final ScoreService scoreService;

  @override
  State<ScoreEntryPage> createState() => _ScoreEntryPageState();
}

class _ScoreEntryPageState extends State<ScoreEntryPage> {
  int _score = 0;
  bool _isSubmitting = false;

  Future<void> _addPoint() async {
    if (_isSubmitting) {
      return;
    }

    setState(() {
      _isSubmitting = true;
    });

    try {
      final updatedScore = await widget.scoreService.submitScore(
        matchId: 'match-1',
        playerId: 'player-1',
        delta: 1,
      );

      if (!mounted) {
        return;
      }

      setState(() {
        _score = updatedScore;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Score updated to $_score'),
          duration: const Duration(milliseconds: 800),
        ),
      );
    } catch (_) {
      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Score update failed. Please retry.'),
          duration: Duration(milliseconds: 1000),
        ),
      );
    } finally {
      if (mounted) {
        setState(() {
          _isSubmitting = false;
        });
      }
    }
  }

  void _subtractPoint() {
    if (_score == 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Score cannot go below 0'),
          duration: Duration(milliseconds: 800),
        ),
      );
      return;
    }

    setState(() {
      _score--;
    });

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Score updated to $_score'),
        duration: const Duration(milliseconds: 800),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Fast Score Entry'),
        centerTitle: true,
      ),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                'Current Score',
                textAlign: TextAlign.center,
                style: theme.textTheme.titleLarge,
              ),
              const SizedBox(height: 8),
              Semantics(
                liveRegion: true,
                label: 'Current score $_score',
                child: Text(
                  '$_score',
                  key: const Key('score-value'),
                  textAlign: TextAlign.center,
                  style: theme.textTheme.displayLarge?.copyWith(
                    fontWeight: FontWeight.w800,
                  ),
                ),
              ),
              const SizedBox(height: 24),
              SizedBox(
                height: 72,
                child: ElevatedButton.icon(
                  key: const Key('add-point-button'),
                  onPressed: _isSubmitting ? null : _addPoint,
                  icon: const Icon(Icons.add_circle_outline, size: 32),
                  label: const Text('Add Point', style: TextStyle(fontSize: 24)),
                ),
              ),
              const SizedBox(height: 16),
              SizedBox(
                height: 72,
                child: OutlinedButton.icon(
                  key: const Key('subtract-point-button'),
                  onPressed: _isSubmitting ? null : _subtractPoint,
                  icon: const Icon(Icons.remove_circle_outline, size: 32),
                  label: const Text('Undo Point', style: TextStyle(fontSize: 24)),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
