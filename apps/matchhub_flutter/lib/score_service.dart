import 'dart:convert';

import 'package:http/http.dart' as http;

abstract class ScoreService {
  Future<int> submitScore({
    required String matchId,
    required String playerId,
    required int delta,
  });
}

class HttpScoreService implements ScoreService {
  HttpScoreService({
    required this.baseUrl,
    required this.bearerToken,
    http.Client? httpClient,
  }) : _httpClient = httpClient ?? http.Client();

  final String baseUrl;
  final String bearerToken;
  final http.Client _httpClient;

  @override
  Future<int> submitScore({
    required String matchId,
    required String playerId,
    required int delta,
  }) async {
    final uri = Uri.parse('$baseUrl/api/matches/$matchId/scores');
    final response = await _httpClient.post(
      uri,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $bearerToken',
      },
      body: jsonEncode({
        'playerId': playerId,
        'score': delta,
        'timestamp': DateTime.now().toUtc().toIso8601String(),
      }),
    );

    if (response.statusCode != 200) {
      throw StateError('Score submission failed with status ${response.statusCode}');
    }

    final map = jsonDecode(response.body) as Map<String, dynamic>;
    final updatedScore = map['updatedScore'];
    if (updatedScore is! int) {
      throw StateError('Missing updatedScore in API response');
    }

    return updatedScore;
  }
}

class FakeScoreService implements ScoreService {
  FakeScoreService({this.start = 0});

  int _current = 0;
  final int start;

  @override
  Future<int> submitScore({
    required String matchId,
    required String playerId,
    required int delta,
  }) async {
    _current = (_current == 0 ? start : _current) + delta;
    return _current;
  }
}
